using FluentValidation;
using WebAPI.DTOs.Requests;

namespace WebAPI.Validators;

public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequestDto>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tên tour không được để trống.");

        RuleFor(x => x.DepartureDate)
            .NotEmpty().WithMessage("Ngày khởi hành không được để trống.")
            .GreaterThan(DateTimeOffset.UtcNow).WithMessage("Ngày khởi hành phải là ngày trong tương lai.");

        RuleFor(x => x.PersonInCharge)
            .NotEmpty().WithMessage("Người phụ trách không được để trống.");

        RuleFor(x => x.TourType)
            .IsInEnum().WithMessage("Loại tour không hợp lệ. Chỉ chấp nhận: FIT, GIT, MICE.");

        RuleFor(x => x.GuestCount)
            .GreaterThan(0).WithMessage("Số lượng khách phải lớn hơn 0.");

        RuleFor(x => x.Details)
            .NotEmpty().WithMessage("Phiếu đề nghị phải có ít nhất 01 dịch vụ đi kèm.")
            .Must(d => d != null && d.Count > 0).WithMessage("Phiếu đề nghị phải có ít nhất 01 dịch vụ đi kèm.");

        RuleForEach(x => x.Details).SetValidator(new CreateRequestDetailValidator());
    }
}

public class CreateRequestDetailValidator : AbstractValidator<CreateRequestDetailDto>
{
    public CreateRequestDetailValidator()
    {
        RuleFor(x => x.ServiceType)
            .NotEmpty().WithMessage("Loại dịch vụ không được để trống.");

        RuleFor(x => x.ServiceName)
            .NotEmpty().WithMessage("Tên dịch vụ không được để trống.");

        RuleFor(x => x.Supplier)
            .NotEmpty().WithMessage("Nhà cung cấp không được để trống.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Đơn giá phải lớn hơn 0.");
    }
}

public class UpdateBookingRequestValidator : AbstractValidator<UpdateBookingRequestDto>
{
    public UpdateBookingRequestValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tên tour không được để trống.");

        RuleFor(x => x.DepartureDate)
            .NotEmpty().WithMessage("Ngày khởi hành không được để trống.");

        RuleFor(x => x.PersonInCharge)
            .NotEmpty().WithMessage("Người phụ trách không được để trống.");

        RuleFor(x => x.TourType)
            .IsInEnum().WithMessage("Loại tour không hợp lệ.");

        RuleFor(x => x.GuestCount)
            .GreaterThan(0).WithMessage("Số lượng khách phải lớn hơn 0.");

        RuleFor(x => x.Details)
            .NotEmpty().WithMessage("Phiếu đề nghị phải có ít nhất 01 dịch vụ đi kèm.");

        RuleForEach(x => x.Details).SetValidator(new CreateRequestDetailValidator());
    }
}
