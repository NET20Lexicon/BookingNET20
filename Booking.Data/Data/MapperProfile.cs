using AutoMapper;
using Booking.Core.Entities;
using Booking.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Booking.Data.Data
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateGymClassViewModel, GymClass>();
            CreateMap<EditGymClassViewModel, GymClass>().ReverseMap();

            CreateMap<GymClass, GymClassesViewModel>()
                .ForMember(dest => dest.Attending, opt => opt.Ignore());

            //CreateMap<GymClass, GymClassesViewModel>()
            //.ForMember(
            //dest => dest.Attending,
            //opt => opt.MapFrom(
            //    (src, dest, _, context) => src.AttendedMembers.Any
            //    (m => m.ApplicationUserId == context.Items["Id"].ToString())));

            //CreateMap<GymClass, GymClassesViewModel>()
            //        .ForMember(
            //        dest => dest.Attending,
            //        opt => opt.MapFrom<AttendingResolver>());

            //CreateMap<IEnumerable<GymClass>, IndexViewModel>()
            //   .ForMember(
            //           dest => dest.GymClasses,
            //           from => from.MapFrom(g => g.ToList())
            //       )
            //   .ForMember(
            //           dest => dest.ShowHistory, opt => opt.Ignore()
            //   );

        }

    }

    //public class AttendingResolver : IValueResolver<GymClass, GymClassesViewModel, bool>
    //{
    //    public bool Resolve(GymClass source, GymClassesViewModel destination, bool destMember, ResolutionContext context)
    //    {
    //        if (source.AttendedMembers == null || context.Items.Count == 0) return false;
    //        return source.AttendedMembers.Any(m => m.ApplicationUserId == context.Items["Id"].ToString());
    //    }
    //}
}
