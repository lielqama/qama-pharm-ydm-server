using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PharmYdm.Models
{
    public class BarQamaClient
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        [Display(Name = "סוג משלוח")]
        public string ShipCode { get; set; }
        [Display(Name = "שם משתמש")]
        public string Username { get; set; }
        [Display(Name = "סיסמא")]
        public string Password { get; set; }
        [Display(Name = "קוד לקוח")]
        public string CustCode { get; set; }
    }
}