using AutoMapper;

using CodingChallenge.Discord.Bot.Models.ChallengeApi;
using CodingChallenge.Discord.Bot.Models.Mongo;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Discord.Bot.Models.Automapper
{
    internal class ChallengeProfile : Profile
    {
        public ChallengeProfile()
        {
            base.CreateMap<ChallengeDiscoveryDto, ChallengeRepositoryData>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}