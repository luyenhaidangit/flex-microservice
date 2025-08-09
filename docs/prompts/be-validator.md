## BE Validator (FluentValidation)
- Viết AbstractValidator<TDto> cho syntax: NotEmpty, Length, Regex, MaxLength.
- KHÔNG gọi DB trừ khi là pre-check UX; nguồn sự thật là unique index ở DB.

### Input
- Dto: {{DtoName}} với fields: {{Fields}}

### Output
- File: {{Project}}/Validators/{{DtoName}}Validator.cs
