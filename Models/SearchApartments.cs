// VIEW MODEL!
// No scaffolding.

using System;
using System.ComponentModel.DataAnnotations;

namespace PSA_JH_YR_AB.Models
{
    public class SearchApartments
    {
        [Required(ErrorMessage = "Please select the number of bedrooms.")]
        [Display(Name = "Number of Bedrooms")]
        public int? NumberOfBedrooms { get; set; }

        [Required(ErrorMessage = "Please enter the Check In date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Check In Date")]
        public DateTime? CheckIn { get; set; }

        [Required(ErrorMessage = "Please enter the Check Out date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Check Out Date")]
        public DateTime? CheckOut { get; set; }
    }
}
