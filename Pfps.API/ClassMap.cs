using AutoMapper;
using Pfps.API.Data;
using Pfps.API.Models;

namespace Pfps.API
{
    public class ClassMap : Profile
    {
        public ClassMap()
        {
            // Use for more advanced class maps later on.

            CreateMap<Upload, UploadViewModel>();
            CreateMap<Notification, NotificationViewModel>();

            CreateMap<User, UserViewModel>()
                .ForMember(d => d.DiscordId,
                f => f.MapFrom(x => x.HasLinkedDiscord ? x.Password : null)); // this might crash
        }
    }
}
