using Domain.Models;
using Infrastructure.Context;
using Infrastructure.Repos.abstracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos.Implementation
{
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

        public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
        {
            return await _context.Subjects
                .Include(s => s.studentSubjects)
                .ToListAsync();
        }

        public async Task AddSubjectAsync(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
        }

        public void UpdateSubject(Subject subject)
        {
            _context.Subjects.Update(subject);
        }

        public void RemoveSubject(Subject subject)
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

        public async Task AddUserSubjectAsync(UserSubject userSubject)
        {
            await _context.UserSubjects.AddAsync(userSubject);
        }

        public void RemoveUserSubject(UserSubject userSubject)
        {
            _context.UserSubjects.Remove(userSubject);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}