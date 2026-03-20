using System.Collections;
using FluentValidation;

public class UpdateDestinationValidator : AbstractValidator<UpdateDestinationRequest>
{
    public UpdateDestinationValidator()
    {
        RuleFor(x => x.Province)
            .NotEmpty().WithMessage("Province is required.")
            .MaximumLength(100).WithMessage("Province cannot exceed 100 characters.")
            .Must(province => ProvinceInVn.Contains(province)).WithMessage("Province must be a valid province in Vietnam.");

    }

    public ArrayList ProvinceInVn = new ArrayList
    {
       "Tuyên Quang",
        "Cao Bằng",
        "Lai Châu",
        "Lào Cai",
        "Thái Nguyên",
        "Điện Biên",
        "Lạng Sơn",
        "Sơn La",
        "Phú Thọ",
        "Bắc Ninh",
        "Quảng Ninh",
        "Hà Nội",
        "Hải Phòng",
        "Hưng Yên",
        "Ninh Bình",
        "Thanh Hóa",
        "Nghệ An",
        "Hà Tĩnh",
        "Quảng Trị",
        "Huế",
        "Đà Nẵng",
        "Quảng Ngãi",
        "Gia Lai",
        "Đắk Lắk",
        "Khánh Hòa",
        "Lâm Đồng",
        "Đồng Nai",
        "Tây Ninh",
        "TP. Hồ Chí Minh",
        "Đồng Tháp",
        "An Giang",
        "Vĩnh Long",
        "Cần Thơ",
        "Cà Mau"
    };
}