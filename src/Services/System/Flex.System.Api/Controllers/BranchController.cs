using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Flex.System.Api.Entities;
using Flex.System.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.System.Branch;
using Flex.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Flex.System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IMapper _mapper;

        public BranchController(IBranchRepository branchRepository, IMapper mapper)
        {
            _branchRepository = branchRepository;
            _mapper = mapper;
        }

        #region Query

        /// <summary>
        /// Phân trang danh sách chi nhánh.
        /// </summary>
        [HttpGet("get-branches-paging")]
        public async Task<IActionResult> GetPagingBranchesAsync([FromQuery] GetBranchesPagingRequest request)
        {
            var query = _branchRepository.FindAll();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword) || x.Code.Contains(request.Keyword));
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(x => x.Status == request.Status);
            }

            var pagedResult = await query.ToPagedResultAsync(request);
            var result = pagedResult.MapPagedResult<Branch, BranchDto>(_mapper);

            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Lấy chi tiết chi nhánh theo ID.
        /// </summary>
        [HttpGet("get-branch-by-id")]
        public async Task<IActionResult> GetByIdAsync([FromQuery] long id)
        {
            var branch = await _branchRepository.FindByIdAsync(id);
            if (branch == null)
                return NotFound(Result.Failure(message: "Branch not found."));

            var dto = _mapper.Map<BranchDto>(branch);
            return Ok(Result.Success(dto));
        }

        #endregion

        #region Command

        /// <summary>
        /// Tạo mới chi nhánh (Pending).
        /// </summary>
        [HttpPost("create-branch")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateBranchRequest request)
        {
            // TODO: có thể check trùng mã chi nhánh nếu cần

            var entity = _mapper.Map<Branch>(request);
            entity.Status = "Pending";
            await _branchRepository.CreateAsync(entity);
            return Ok(Result.Success());
        }

        /// <summary>
        /// Cập nhật chi nhánh (sẽ tạo yêu cầu phê duyệt nếu quy trình đầy đủ).
        /// </summary>
        [HttpPost("update-branch")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateBranchRequest request)
        {
            var entity = await _branchRepository.FindByIdAsync(request.Id);
            if (entity == null)
                return NotFound(Result.Failure(message: "Branch not found."));

            _mapper.Map(request, entity);
            await _branchRepository.UpdateAsync(entity);
            return Ok(Result.Success());
        }

        /// <summary>
        /// Đóng (Deactivate) chi nhánh.
        /// </summary>
        [HttpPost("close-branch")]
        public async Task<IActionResult> CloseAsync([FromBody] long id)
        {
            var entity = await _branchRepository.FindByIdAsync(id);
            if (entity == null)
                return NotFound(Result.Failure(message: "Branch not found."));

            entity.Status = "Inactive";
            await _branchRepository.UpdateAsync(entity);
            return Ok(Result.Success());
        }

        #endregion
    }
}
