using AutoMapper;
using Flex.Infrastructure.EF;
using Flex.Investor.Api.Repositories.Interfaces;
using Flex.Investor.Api.Services.Interfaces;
using Flex.Shared.DTOs.Investor;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flex.Investor.Api.Services
{
    public class InvestorService : IInvestorService
    {
        private readonly IInvestorRepository _investorRepository;
        private readonly IMapper _mapper;

        public InvestorService(IInvestorRepository investorRepository, IMapper mapper)
        {
            _investorRepository = investorRepository;
            _mapper = mapper;
        }

        #region Query
        /// <summary>
        /// Phân trang Nhà đầu tư.
        /// </summary>
        public async Task<IResult> GetPagingInvestorsAsync(GetInvestorsPagingRequest request)
        {
            var query = _investorRepository.FindAll()
                .WhereIf(!string.IsNullOrEmpty(request.No), b => b.No.ToUpper().Contains(request.No.ToUpper()))
                .WhereIf(!string.IsNullOrEmpty(request.Name), b => b.FullName.ToUpper().Contains(request.Name.ToUpper()));

            var resultPaged = await query.ToPagedResultAsync(request);

            var resultDtoPaged = resultPaged.MapPagedResult<Entities.Investor, InvestorDto>(_mapper);

            return Results.Ok(Result.Success(resultDtoPaged));
        }

        /// <summary>
        /// Thông tin Chi tiết nhà đầu tư.
        /// </summary>
        public async Task<IResult> GetInvestorByIdAsync([FromQuery] EntityKey<long> entityKey)
        {
            var securities = await _investorRepository.FindByCondition(x => x.Id == entityKey.Id).FirstOrDefaultAsync();
            if (securities is null)
            {
                return Results.BadRequest(Result.Failure(message: "Securities not found."));
            }

            var result = _mapper.Map<InvestorDto>(securities);

            return Results.Ok(Result.Success(result));
        }
        #endregion

        #region Command
        /// <summary>
        /// Thêm mới Nhà đầu tư.
        /// </summary>
        //public async Task<Result> CreateInvestorAsync(CreateInvestorRequest request)
        //{
        //    // Check if No already exists
        //    var isExist = await _investorRepository.FindByCondition(x => x.No.ToUpper() == request.No.ToUpper()).AnyAsync();
        //    if (isExist)
        //    {
        //        return Result.Failure(message: "Investor with the provided No already exists.");
        //    }

        //    var investor = _mapper.Map<Entities.Investor>(request);

        //    await _investorRepository.CreateAsync(investor);

        //    return Result.Success();
        //}

        /// <summary>
        /// Cập nhật Nhà đầu tư.
        /// </summary>
        //public async Task<Result> UpdateInvestorAsync(UpdateInvestorRequest request)
        //{
        //    try
        //    {
        //        var investor = await _investorRepository.FindByCondition(x => x.Id == request.Id).FirstOrDefaultAsync();
        //        if (investor is null)
        //        {
        //            return Result.Failure(message: "Investor not found.");
        //        }

        //        // Kiểm tra tồn tại No nếu có thay đổi
        //        if (!string.Equals(investor.No, request.No, StringComparison.OrdinalIgnoreCase))
        //        {
        //            var isNoExist = await _investorRepository.FindByCondition(x => x.No.ToUpper() == request.No.ToUpper() && x.Id != request.Id).AnyAsync();
        //            if (isNoExist)
        //            {
        //                _logger.Warning("Investor with No: {InvestorNo} already exists.", request.No);
        //                return Result.Failure(message: "Another investor with the provided No already exists.");
        //            }
        //        }

        //        _mapper.Map(request, investor);
        //        investor.UpdatedAt = DateTime.UtcNow;

        //        await _investorRepository.UpdateAsync(investor);

        //        _logger.Information("Updated investor with ID: {InvestorId}", investor.Id);

        //        return Result.Success(message: "Investor updated successfully.", data: _mapper.Map<InvestorDto>(investor));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex, "Error occurred while updating investor with ID: {InvestorId}", request.Id);
        //        return Result.Failure(message: "An error occurred while updating the investor.");
        //    }
        //}

        /// <summary>
        /// Xoá Nhà đầu tư.
        /// </summary>
        //public async Task<Result> DeleteInvestorAsync(long id)
        //{
        //    try
        //    {
        //        var investor = await _investorRepository.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
        //        if (investor is null)
        //        {
        //            _logger.Warning("Investor with ID: {InvestorId} not found for deletion.", id);
        //            return Result.Failure(message: "Investor not found.");
        //        }

        //        await _investorRepository.DeleteAsync(investor);

        //        _logger.Information("Deleted investor with ID: {InvestorId}", id);

        //        return Result.Success(message: "Investor deleted successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex, "Error occurred while deleting investor with ID: {InvestorId}", id);
        //        return Result.Failure(message: "An error occurred while deleting the investor.");
        //    }
        //}

        Task<PagedResult<InvestorDto>> IInvestorService.GetPagingInvestorsAsync(GetInvestorsPagingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<InvestorDto> GetInvestorByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Result> CreateInvestorAsync(CreateInvestorRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateInvestorAsync(UpdateInvestorRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteInvestorAsync(long id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
