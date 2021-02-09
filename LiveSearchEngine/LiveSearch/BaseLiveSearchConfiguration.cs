using System.Collections.Generic;
using LiveSearchEngine.Interfaces;

namespace LiveSearchEngine.LiveSearch
{
    public abstract class BaseLiveSearchConfiguration
    {
        public IReadOnlyList<ISniperItemValidator> Validators => _validators.AsReadOnly();

        public void AddValidator(ISniperItemValidator validator) => _validators.Add(validator);
        public void RemoveValidator(ISniperItemValidator validator) => _validators.Remove(validator);

        readonly List<ISniperItemValidator> _validators = new List<ISniperItemValidator>();
    }
}