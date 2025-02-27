using System.Collections.Generic;
using FluentValidation;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.CustomFormats
{
    public class SizeSpecificationValidator : AbstractValidator<SizeSpecification>
    {
        public SizeSpecificationValidator()
        {
            RuleFor(c => c.Min).GreaterThanOrEqualTo(0);
            RuleFor(c => c.Max).GreaterThan(c => c.Min);
        }
    }

    public class SizeSpecification : CustomFormatSpecificationBase
    {
        private static readonly SizeSpecificationValidator Validator = new SizeSpecificationValidator();

        public override int Order => 8;
        public override string ImplementationName => "Size";

        [FieldDefinition(1, Label = "Minimum Size", HelpText = "Release must be greater than this size", Unit = "GB", Type = FieldType.Number)]
        public double Min { get; set; }

        [FieldDefinition(1, Label = "Maximum Size", HelpText = "Release must be less than or equal to this size", Unit = "GB", Type = FieldType.Number)]
        public double Max { get; set; }

        protected override bool IsSatisfiedByWithoutNegate(ParsedMovieInfo movieInfo)
        {
            var size = (movieInfo?.ExtraInfo?.GetValueOrDefault("Size", 0.0) as long?) ?? 0;

            return size > Min.Gigabytes() && size <= Max.Gigabytes();
        }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
