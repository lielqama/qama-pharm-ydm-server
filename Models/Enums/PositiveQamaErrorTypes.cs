using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PharmYdm.Models.Enums
{
    public enum PharmYdmQamaErrorTypes
    {
        [Display(Name = "Object casting fail")]
        ConvertObject,
        [Display(Name = "Http Exeption (400)")]
        Http,
        [Display(Name = "Client Don't exsist or token missing")]
        Auth,
        [Display(Name = "PharmYdm bussines logic error")]
        PharmYdmLogic,
        [Display(Name = "Priority bussines logic error")]
        PriorityLogic,
        [Display(Name = "Other")]
        Other
    }
}