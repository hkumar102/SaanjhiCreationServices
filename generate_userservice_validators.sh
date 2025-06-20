#!/bin/bash

echo "🧪 Generating FluentValidators for UserService..."

BASE="./services/UserService/UserService.Application/Validators"

mkdir -p "$BASE"

# CreateUserCommandValidator.cs
cat > "$BASE/CreateUserCommandValidator.cs" <<EOF
using FluentValidation;
using UserService.Application.Commands;

namespace UserService.Application.Validators;

/// <summary>
/// Validator for CreateUserCommand.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirebaseUserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
EOF

# UpdateUserCommandValidator.cs
cat > "$BASE/UpdateUserCommandValidator.cs" <<EOF
using FluentValidation;
using UserService.Application.Commands;

namespace UserService.Application.Validators;

/// <summary>
/// Validator for UpdateUserCommand.
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
EOF

# AddShippingAddressCommandValidator.cs
cat > "$BASE/AddShippingAddressCommandValidator.cs" <<EOF
using FluentValidation;
using UserService.Application.Commands;

namespace UserService.Application.Validators;

/// <summary>
/// Validator for AddShippingAddressCommand.
/// </summary>
public class AddShippingAddressCommandValidator : AbstractValidator<AddShippingAddressCommand>
{
    public AddShippingAddressCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AddressLine1).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.State).NotEmpty();
        RuleFor(x => x.PostalCode).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
    }
}
EOF

echo "✅ Validators created for CreateUser, UpdateUser, and AddShippingAddress."
