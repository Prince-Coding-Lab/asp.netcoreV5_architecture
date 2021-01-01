using AutoMapper;
using ScaffoldNetCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScaffoldNetCore.Core.Dto
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserCreateDto,Users>();
           
        }
    }
}
