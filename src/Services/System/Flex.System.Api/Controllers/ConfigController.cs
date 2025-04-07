using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Flex.System.Api.Entities;
using Flex.System.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.System;
using Flex.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;
using Flex.Infrastructure.EF;
using Flex.Shared.Constants;

namespace Flex.System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigRepository _configRepository;
        private readonly IMapper _mapper;

        public ConfigController(IConfigRepository configRepository, IMapper mapper)
        {
            _configRepository = configRepository;
            _mapper = mapper;
        }

        #region Query
        /// <summary>
        /// Phân trang danh sách cấu hình.
        /// </summary>
        [HttpGet("get-configs-paging")]
        public async Task<IActionResult> GetPagingConfigsAsync([FromQuery] GetConfigsPagingRequest request)
        {
            var query = _configRepository.FindAll();

            // Nếu có điều kiện lọc theo Key
            if (!string.IsNullOrEmpty(request.Key))
            {
                query = query.Where(c => c.Key.Contains(request.Key));
            }

            var pagedResult = await query.ToPagedResultAsync(request);
            var pagedResultDto = pagedResult.MapPagedResult<Config, ConfigDto>(_mapper);

            return Ok(Result.Success(pagedResultDto));
        }

        /// <summary>
        /// Lấy thông tin cấu hình theo Key.
        /// </summary>
        [HttpGet("get-config-by-key")]
        public async Task<IActionResult> GetConfigByKeyAsync([FromQuery] string key)
        {
            var config = await _configRepository.FindByCondition(c => c.Key == key).FirstOrDefaultAsync();
            if (config == null)
            {
                return NotFound(Result.Failure(message: "Config not found."));
            }

            var result = config.Value;

            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Lấy danh sách cấu hình theo danh sách các Key.
        /// Ví dụ: GET api/config/get-configs-by-keys?keys=KEY1&keys=KEY2
        /// </summary>
        [HttpGet("get-configs-by-keys")]
        public async Task<IActionResult> GetConfigsByKeysAsync([FromQuery] List<string> keys)
        {
            if (keys == null || keys.Count == 0)
            {
                return BadRequest(Result.Failure(message: "No keys provided."));
            }

            var configs = await _configRepository.FindByCondition(c => keys.Contains(c.Key)).ToListAsync();
            var configDtos = _mapper.Map<List<ConfigDto>>(configs);
            return Ok(Result.Success(configDtos));
        }

        /// <summary>
        /// Lấy danh sách cấu hình theo danh sách các Key.
        /// Ví dụ: GET api/config/get-configs-by-keys?keys=KEY1&keys=KEY2
        /// </summary>
        [HttpGet("get-auth-mode")]
        public async Task<IActionResult> GetAuthModeAsync()
        {
            var config = await _configRepository.FindByCondition(c => c.Key == ConfigKeyConstants.AuthMode).FirstOrDefaultAsync();
            if (config == null)
            {
                return NotFound(Result.Failure(message: "Config not found."));
            }

            var result = config.Value;

            return Ok(Result.Success(message: result));
        }
        #endregion

        #region Command
        /// <summary>
        /// Thêm mới cấu hình.
        /// </summary>
        [HttpPost("create-config")]
        public async Task<IActionResult> CreateConfigAsync([FromBody] CreateConfigRequest request)
        {
            // Kiểm tra tồn tại theo Key
            bool isExist = await _configRepository.FindByCondition(c => c.Key == request.Key).AnyAsync();
            if (isExist)
            {
                return Conflict(Result.Failure(message: "Config key already exists."));
            }

            var config = _mapper.Map<Config>(request);
            await _configRepository.CreateAsync(config);
            return Ok(Result.Success());
        }

        /// <summary>
        /// Cập nhật cấu hình.
        /// </summary>
        [HttpPost("update-config")]
        public async Task<IActionResult> UpdateConfigAsync([FromBody] UpdateConfigRequest request)
        {
            var config = await _configRepository.FindByCondition(c => c.Key == request.Key).FirstOrDefaultAsync();
            if (config == null)
            {
                return NotFound(Result.Failure(message: "Config not found."));
            }

            _mapper.Map(request, config);
            await _configRepository.UpdateAsync(config);
            return Ok(Result.Success());
        }

        /// <summary>
        /// Xoá cấu hình.
        /// </summary>
        [HttpPost("delete-config")]
        public async Task<IActionResult> DeleteConfigAsync([FromBody] string key)
        {
            var config = await _configRepository.FindByCondition(c => c.Key == key).FirstOrDefaultAsync();
            if (config == null)
            {
                return NotFound(Result.Failure(message: "Config not found."));
            }

            await _configRepository.DeleteAsync(config);
            return Ok(Result.Success());
        }
        #endregion
    }
}
