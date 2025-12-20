using AutoMapper;
using Ids.Models;
using idsProject.Dtos.Answer;


namespace idsProject.Mapping
{
    public class AnswerProfile : Profile
    {
        public AnswerProfile()
        {

            CreateMap<Answer, AnswerResponseDto>();
            CreateMap<AnswerCreateDto, Answer>();
            CreateMap<AnswerUpdateDto, Answer>();
        }
    }
}