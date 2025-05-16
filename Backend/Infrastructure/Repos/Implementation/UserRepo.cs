using Domain.Models;
using Infrastructure.Context;
using Infrastructure.Repos.abstracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos.Implementation
{
    public class GetSubjectsDetails
    {
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public int TotalSubjects { get; set; }
    }
    public class UserRepo : GenericRepo<User>, IUserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            return await _context.Subjects
                .Include(s => s.studentSubjects)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Subject> GetSubjectByNameAsync(string name)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(s => s.Name == name);
        }

        public IQueryable<Subject> GetSubjectsQueryable()
        {
            return _context.Subjects
                .Include(s => s.studentSubjects)
                .AsQueryable();
        }

        public async Task<Subject> GetSubjectWithStudentsAsync(int subjectId)
        {
            return await _context.Subjects
                .Include(s => s.studentSubjects)
                .ThenInclude(us => us.User)
                .FirstOrDefaultAsync(s => s.Id == subjectId);
        }

        public async Task<GetSubjectsDetails> GetAllSubjectsAsync(int studentId, int pageSize, int pageNumber)
        {
            try
            {
                if (studentId <= 0) throw new ArgumentException("Invalid student ID", nameof(studentId));
                if (pageSize <= 0) pageSize = 10;
                if (pageNumber <= 0) pageNumber = 1;

                var query = _context.Subjects
                    .Where(s => !s.studentSubjects.Any(ss => ss.UserId == studentId));

                var subjects = await query
                    .OrderBy(s => s.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalSubjects = await query.CountAsync();

                return new GetSubjectsDetails
                {
                    Subjects = subjects,
                    TotalSubjects = totalSubjects
                };
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error fetching subjects for studentId {StudentId}", studentId);
                throw;
            }
        }

        public async Task AddSubjectAsync(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
        }

        public void UpdateSubject(Subject subject)
        {
            _context.Subjects.Update(subject);
        }

        public async Task RemoveSubject(Subject subject)
        {
            _context.Subjects.Remove(subject);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<UserSubject> GetUserSubjectAsync(int userId, int subjectId)
        {
            return await _context.UserSubjects
                .FirstOrDefaultAsync(us => us.UserId == userId && us.SubjectId == subjectId);
        }
        private async Task<int> TotalCourses()
        {
            return await _context.Subjects.CountAsync();
        }
        public async Task<IEnumerable<Subject>> GetUserSubjectsAsync(int userId)
        {

            return await _context.UserSubjects
                .Where(us => us.UserId == userId)
                .Include(us => us.Subject)
                .OrderBy(us => us.Subject.Id)
                .Select(us => us.Subject)
                .ToListAsync();
        }
        public async Task AddUserSubjectAsync(UserSubject userSubject)
        {
            await _context.UserSubjects.AddAsync(userSubject);
        }

        public async Task RemoveUserSubject(UserSubject userSubject)
        {
            _context.UserSubjects.Remove(userSubject);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}