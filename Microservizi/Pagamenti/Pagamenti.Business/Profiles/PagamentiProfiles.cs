using AutoMapper;
using Pagamenti.Repository.Model;
using Pagamenti.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Pagamenti.Business.Profiles
{
    public sealed class AssemblyMarker
    {
        AssemblyMarker() { }
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public class PagamentiProfiles : Profile
    {
        public PagamentiProfiles()
        {
            CreateMap<VersamentoInsertDto, Versamento>();
            CreateMap<Versamento, VersamentoInsertDto>();
            CreateMap<VersamentoReadDto, Versamento>();
            CreateMap<Versamento, VersamentoReadDto>();
        }
    }
}
