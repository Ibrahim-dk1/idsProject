using AutoMapper;
using Ids.Models;
using idsProject.Dtos.Question;

namespace idsProject.Mapping
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<Question, QuestionResponseDto>();
            CreateMap<QuestionCreateDto, Question>();
            CreateMap<QuestionUpdateDto, Question>();
        }
    }
}
