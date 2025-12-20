using AutoMapper;
using Ids.Models;
using idsProject.Dtos.StudentAnswer;

namespace idsProject.Mapping
{
    public class StudentAnswerProfile : Profile
    {
        public StudentAnswerProfile()
        {
            CreateMap<StudentAnswer, StudentAnswerResponseDto>();
            CreateMap<StudentAnswerCreateDto, StudentAnswer>();
            CreateMap<StudentAnswerUpdateDto, StudentAnswer>();
        }
    }
}
