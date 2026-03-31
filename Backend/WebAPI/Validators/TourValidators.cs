using FluentValidation;
using WebAPI.DTOs.Requests;

namespace WebAPI.Validators;

public class CreateTourValidator : AbstractValidator<CreateTourRequest>
{
    public CreateTourValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên tour không được để trống.")
            .MaximumLength(200).WithMessage("Tên tour không được vượt quá 200 ký tự.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Giá tour phải lớn hơn 0.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Thành phố không được để trống.")
            .MaximumLength(100).WithMessage("Tên thành phố không được vượt quá 100 ký tự.");
    }
}

public class UpdateTourValidator : AbstractValidator<UpdateTourRequest>
{
    public UpdateTourValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên tour không được để trống.")
            .MaximumLength(200).WithMessage("Tên tour không được vượt quá 200 ký tự.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Giá tour phải lớn hơn 0.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Thành phố không được để trống.")
            .MaximumLength(100).WithMessage("Tên thành phố không được vượt quá 100 ký tự.");
    }
}
