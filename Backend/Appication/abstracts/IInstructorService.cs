using Application.Bases;
using Application.DTOs.User;
using Application.DTOs.User.Application.DTOs.Subject;
using Domain.Models;
using Infrastructure.Repos.Implementation;

namespace Application.abstracts
{
    public interface IInstructorService
    {
        IQueryable<ShowSubjectDTO> GetSubjectsQueryable();
        Task<SubjectStudentsDTO> GetSubjectWithStudentsAsync(int subjectId);
        Task<GetSubjectsDetails> GetAllSubjectsAsync(int id, PAginatedDto request);
        Task AddSubjectAsync(AddCourseDTO subject);
        Task<Response<string>> UpdateSubject(UpdateSubjectDTO subject);
        void RemoveSubject(RemoveStudentDTO subject);

        Task<Response<IEnumerable<Subject>>> GetUserSubjectsAsync(int userId);
        Task AddUserSubjectAsync(AssignStudentDTO userSubject);
        Task RemoveUserSubject(RemoveStudentDTO userSubject);
        Task SaveChangesAsync();

    }
}
