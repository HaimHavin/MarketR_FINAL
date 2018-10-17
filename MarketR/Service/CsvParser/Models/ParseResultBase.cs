using System.Collections.Generic;

namespace MarketR.Service.CsvParser.Models
{
    public abstract class ParseResultBase
    {
        protected ParseResultBase()
        {
            this.Errors = new Dictionary<string, List<string>>();
            IsValid = true;
        }

        public bool IsValid { get; set; }

        public Dictionary<string, List<string>> Errors { get; set; }

        public virtual void AddError(string key, List<string> errors)
        {
            if (!Errors.ContainsKey(key))
            {
                Errors.Add(key, errors);
            }

            Errors[key] = errors;

            this.IsValid = false;
        }

        public virtual void AddError(string key, string errorMessage)
        {
            this.AddError(key, new List<string>() { errorMessage });
        }

        public virtual void AddErrors(Dictionary<string, List<string>> errors)
        {
            foreach (var error in errors)
            {
                this.AddError(error.Key, error.Value);
            }
        }
    }
}
