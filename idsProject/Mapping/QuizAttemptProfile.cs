using AutoMapper;
using Ids.Models;
using idsProject.Dtos.QuizAttemptDto;

public class QuizAttemptProfile : Profile
{
    public QuizAttemptProfile()
    {
        CreateMap<QuizAttempt, QuizAttemptResponse>();
        CreateMap<QuizAttemptCreate, QuizAttempt>();
        CreateMap<QuizAttemptUpdate, QuizAttempt>();
    }
}