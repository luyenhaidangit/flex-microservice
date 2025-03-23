using AutoMapper;
using Flex.Infrastructure.EF;
using Flex.Shared.DTOs.System.Department;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.General;
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
        private readonly IDepartmentRequestRepository _departmentRequestRepository;
        private readonly IMapper _mapper;

        public DepartmentController(IDepartmentRepository departmentRepository, IDepartmentRequestRepository departmentRequestRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _departmentRequestRepository = departmentRequestRepository;
            _mapper = mapper;
        }

        #region Query
        /// <summary>
        /// Phân trang Danh sách phòng ban.
        /// </summary>
        [HttpGet("get-department-paging")]
        public async Task<IActionResult> GetPagingDepartmentsAsync([FromQuery] GetDepartmentsPagingRequest request)
        {   // Lấy danh sách request và main
            // Lọc theo điều kiện và phân trang
            string defaultCreateStatus = "P";

            var departmentsQuery = _departmentRepository.FindAll()
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Address = d.Address,
                    Description = d.Description,
                    Status = d.Status,
                    CreatedDate = d.CreatedDate
                });

            var departmentRequestsQuery = _departmentRequestRepository.FindAll()
                .Where(r => r.ActionType == "CREATE" && r.RequestStatus != "PENDING")
                .Select(r => new DepartmentDto
                {
                    Id = null,
                    Name = r.Name,
                    Address = r.Address,
                    Description = r.Description,
                    Status = defaultCreateStatus,
                    CreatedDate = r.CreatedDate
                });

            var combinedQuery = departmentsQuery.Concat(departmentRequestsQuery);

            var query = combinedQuery
                .WhereIf(!string.IsNullOrEmpty(request.Name), b => b.Name.ToUpper().Contains(request.Name.ToUpper()))
                .WhereIf(!string.IsNullOrEmpty(request.Status), b => b.Status.ToUpper().Contains(request.Status.ToUpper()));

            var resultPaged = await query.ToPagedResultAsync(request);

            var resultDtoPaged = resultPaged.MapPagedResult<DepartmentDto, DepartmentDto>(_mapper);

            return Ok(Result.Success(resultDtoPaged));
        }

        [HttpPost("create-department")]
        public async Task<IActionResult> CreateSecuritieAsync([FromBody] CreateDepartmentRequest request)
        {
            var createDepartmentRequest = _mapper.Map<DepartmentRequest>(request);
            createDepartmentRequest.ActionType = "CREATE";
            createDepartmentRequest.RequestStatus = "PENDING";

            await _departmentRequestRepository.CreateAsync(createDepartmentRequest);

            // Result

            return Ok(Result.Success());
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

        /// <summary>
        /// Duyệt Tổ chức phát hành.
        /// </summary>
        [HttpPost("approve-department")]
        public async Task<IActionResult> ApproveOrRejectRequestAsync([FromBody] ApproveOrRejectRequest<long> request)
        {
            if (request.ActionType == "CREATE")
            {
                // Validate
                var isExistRequests = await _departmentRequestRepository.FindByCondition(x => x.Id == request.Id).AnyAsync();

                if (!isExistRequests)
                {
                    return BadRequest(Result.Failure(message: "Department request not found."));
                }

                var departmentRequest = await _departmentRequestRepository.FindByCondition(x => x.Id == request.Id).FirstAsync();
                if (departmentRequest.ActionType != "CREATE" || departmentRequest.RequestStatus != "PENDING")
                {
                    return BadRequest(Result.Failure(message: "Issuer request is not pending create."));
                }

                // Begin: Transaction
                var transaction = _departmentRepository.BeginTransactionAsync();

                // Create issuers
                var department = _mapper.Map<Department>(departmentRequest);
                department.ProcessStatus = "CA";
                _departmentRepository.Create(department);

                await _departmentRepository.EndTransactionAsync();
                // End: Transaction
            }
            else if (request.ActionType == "EDIT")
            {
                //// Validate
                //var isExistRequests = await _issuerRequestRepository.FindByCondition(x => x.Id == request.Id).AnyAsync();

                //if (!isExistRequests)
                //{
                //    return BadRequest(Result.Failure(message: "Issuer request not found."));
                //}

                //// Begin: Transaction
                //var transaction = _issuerRepository.BeginTransactionAsync();

                //var issuerRequest = await _issuerRequestRepository.FindByCondition(x => x.Id == request.Id).FirstAsync();

                //// Update issuers
                //var issuer = _mapper.Map<CatalogIssuer>(issuerRequest);
                //issuer.ProcessStatus = EProcessStatus.Complete;
                //_issuerRepository.Update(issuer);

                //// Delete add request
                //_issuerRequestRepository.Delete(issuerRequest);

                //await _issuerRepository.EndTransactionAsync();
                //// End: Transaction
            }
            else if (request.ActionType == "DELETE")
            {
                //// Validate
                //var isExistEntity = await _issuerRepository.FindByCondition(x => x.Id == request.Id).AnyAsync();
                //if (!isExistEntity)
                //{
                //    return BadRequest(Result.Failure(message: "Issuer not found."));
                //}

                //// Process
                //var issuer = await _issuerRepository.FindByCondition(x => x.Id == request.Id).FirstAsync();
                //await _issuerRepository.DeleteAsync(issuer);
                //// End: Transaction
            }

            // Result

            return Ok(Result.Success());
        }
        #endregion
    }
}
