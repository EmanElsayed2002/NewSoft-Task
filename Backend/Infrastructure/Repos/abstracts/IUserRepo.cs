using Domain.Models;

namespace Infrastructure.Repos.abstracts
{
    public interface IUserRepo : IGenericRepo<User>
    {
        Task<Subject> GetSubjectByIdAsync(int id);
        Task<Subject> GetSubjectByNameAsync(string name);
        Task<IEnumerable<Subject>> GetAllSubjectsAsync();
        Task AddSubjectAsync(Subject subject);
        void UpdateSubject(Subject subject);
        void RemoveSubject(Subject subject);


        IQueryable<Subject> GetSubjectsQueryable();
        Task<Subject> GetSubjectWithStudentsAsync(int subjectId);



        Task<User> GetUserByIdAsync(int id);

        Task<UserSubject> GetUserSubjectAsync(int userId, int subjectId);
        Task AddUserSubjectAsync(UserSubject userSubject);
        void RemoveUserSubject(UserSubject userSubject);


        Task SaveChangesAsync();
    }
}
