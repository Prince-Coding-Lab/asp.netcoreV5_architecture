﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ScaffoldNetCore.Core.Dto
{
    public class UserCreateDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? RoleId { get; set; }
    }
}
