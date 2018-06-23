using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BeltExam.Models
{
    public class ValidPlan
    {
        [Required(ErrorMessage="Event title is required")]
        [MinLength(2, ErrorMessage="Event title must be at least 2 characters")]
        public string Title { get; set; }
        [FutureDate(ErrorMessage="Must be in the future")]
        public DateTime Start { get; set; }
        [Required]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage="Must enter a valid number")]
        public string Duration { get; set; }
        public string Unit { get; set; }
        [MinLength(10, ErrorMessage="Description must be at least 10 characters")]
        public string Description { get; set; }
    }
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool valid = false;
                if((DateTime)value > DateTime.Now)
                {
                    valid = true;
                }
            return valid;
        }
    }
}