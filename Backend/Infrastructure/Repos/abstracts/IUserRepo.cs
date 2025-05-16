using Domain.Models;
using Infrastructure.Repos.Implementation;

namespace Infrastructure.Repos.abstracts
{
    public interface IUserRepo : IGenericRepo<User>
    {
        Task<Subject> GetSubjectByIdAsync(int id);
        Task<Subject> GetSubjectByNameAsync(string name);
        Task<GetSubjectsDetails> GetAllSubjectsAsync(int studentId, int pageSize, int pageNumber);
        Task AddSubjectAsync(Subject subject);
        void UpdateSubject(Subject subject);
        Task RemoveSubject(Subject subject);


        IQueryable<Subject> GetSubjectsQueryable();
        Task<Subject> GetSubjectWithStudentsAsync(int subjectId);



        Task<User> GetUserByIdAsync(int id);

        Task<UserSubject> GetUserSubjectAsync(int userId, int subjectId);
        Task<IEnumerable<Subject>> GetUserSubjectsAsync(int userId);
        Task AddUserSubjectAsync(UserSubject userSubject);
        Task RemoveUserSubject(UserSubject userSubject);


        Task SaveChangesAsync();
    }
}
