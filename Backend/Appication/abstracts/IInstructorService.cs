using Application.Bases;
using Application.DTOs.User.Application.DTOs.Subject;

namespace Application.abstracts
{
    public interface IInstructorService
    {
        Task<ShowSubjectDTO> GetSubjectByIdAsync(int id);
        Task<ShowSubjectDTO> GetSubjectByNameAsync(string name);
        IQueryable<ShowSubjectDTO> GetSubjectsQueryable();
        Task<SubjectStudentsDTO> GetSubjectWithStudentsAsync(int subjectId);
        //public Task<Response<PagedList<ShowSubjectDTO>>> GetAllSubjects();
        Task<IEnumerable<ShowSubjectDTO>> GetAllSubjectsAsync();
        Task AddSubjectAsync(AddCourseDTO subject);
        Task<Response<string>> UpdateSubject(UpdateSubjectDTO subject);
        void RemoveSubject(RemoveStudentDTO subject);

        Task<UserInfoDTO> GetUserByIdAsync(int id);

        Task<SubjectStudentsDTO> GetUserSubjectAsync(int userId, int subjectId);
        Task AddUserSubjectAsync(AssignStudentDTO userSubject);
        void RemoveUserSubject(RemoveStudentDTO userSubject);

        Task SaveChangesAsync();

    }
}
