using AutoMapper;
using Flex.Infrastructure.EF;
using Flex.Shared.DTOs.System.Department;
using Flex.Shared.SeedWork;
using Flex.System.Api.Entities;
using Flex.System.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flex.System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public DepartmentController(IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        #region Query
        /// <summary>
        /// Phân trang Danh sách phòng ban.
        /// </summary>
        [HttpGet("get-department-paging")]
        public async Task<IActionResult> GetPagingSecuritiesAsync([FromQuery] GetDepartmentsPagingRequest request)
        {
            var query = _departmentRepository.FindAll().WhereIf(!string.IsNullOrEmpty(request.Key), b => b.Key.ToUpper().Contains(request.Key.ToUpper()));

            var resultPaged = await query.ToPagedResultAsync(request);

            var resultDtoPaged = resultPaged.MapPagedResult<Department, DepartmentDto>(_mapper);

            return Ok(Result.Success(resultDtoPaged));
        }

        /// <summary>
        /// Thông tin Chi tiết phòng ban.
        /// </summary>
        [HttpGet("get-security-by-id")]
        public async Task<IActionResult> GetSecuritiesByIdAsync([FromQuery] EntityKey<long> entityKey)
        {
            var department = await _departmentRepository.FindByCondition(x => x.Id == entityKey.Id).FirstOrDefaultAsync();
            if (department is null)
            {
                return BadRequest(Result.Failure(message: "Securities not found."));
            }

            var result = _mapper.Map<DepartmentDto>(department);

            return Ok(Result.Success(result));
        }
        #endregion
    }
}
