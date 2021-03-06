﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Library.API.Helpers;

namespace Library.API.Models
{
    public class BookForCreationDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        [NotEqualTo("Title", ErrorMessage = "Description should be different to title.")]
        public virtual string Description { get; set; }
    }
}
