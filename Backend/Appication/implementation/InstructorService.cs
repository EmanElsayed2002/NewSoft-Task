using Application.abstracts;
using Application.Bases;
using Application.DTOs.User;
using Application.DTOs.User.Application.DTOs.Subject;
using Domain.Models;
using Infrastructure.Repos.abstracts;
using Infrastructure.Repos.Implementation;

namespace Application.Implementation
{
    public class InstructorService : IInstructorService
    {
        private readonly IUserRepo _repo;
        private readonly IResponseHandler _handler;

        public InstructorService(IUserRepo userRepo, IResponseHandler responseHandler)
        {
            _repo = userRepo;
            _handler = responseHandler;
        }

        #region Interface Implementation


        public IQueryable<ShowSubjectDTO> GetSubjectsQueryable()
        {
            return _repo.GetSubjectsQueryable().Select(s => MapToShowSubjectDTO(s));
        }

        public async Task<SubjectStudentsDTO> GetSubjectWithStudentsAsync(int subjectId)
        {
            var subject = await _repo.GetSubjectWithStudentsAsync(subjectId);
            return subject == null ? null : MapToSubjectStudentsDTO(subject);
        }

        public async Task<GetSubjectsDetails> GetAllSubjectsAsync(int Id, PAginatedDto request)
        {
            var subjects = await _repo.GetAllSubjectsAsync(Id, request.PageSize, request.PageNumber);
            return subjects;
        }

        public async Task AddSubjectAsync(AddCourseDTO subjectDto)
        {
            var subject = new Subject
            {
                Name = subjectDto.Name,
                studentSubjects = new List<UserSubject>()
            };
            await _repo.AddSubjectAsync(subject);
            await _repo.SaveChangesAsync();
        }



        public async void RemoveSubject(RemoveStudentDTO removeDto)
        {
            var subject = await _repo.GetSubjectByIdAsync(removeDto.SubjectId);
            if (subject != null)
            {
                await _repo.RemoveSubject(subject);
                await _repo.SaveChangesAsync();
            }
        }

        public async Task<UserInfoDTO> GetUserByIdAsync(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            return user == null ? null : new UserInfoDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Address = user.Address,
                Phone = user.PhoneNumber,
                Age = user.Age
            };
        }



        public async Task AddUserSubjectAsync(AssignStudentDTO assignDto)
        {
            var userSubject = new UserSubject
            {
                UserId = assignDto.StudentId,
                SubjectId = assignDto.SubjectId
            };
            await _repo.AddUserSubjectAsync(userSubject);
            await _repo.SaveChangesAsync();
        }

        public async Task RemoveUserSubject(RemoveStudentDTO removeDto)
        {
            var userSubject = await _repo.GetUserSubjectAsync(removeDto.StudentId, removeDto.SubjectId);
            if (userSubject != null)
            {
                await _repo.RemoveUserSubject(userSubject);
                await _repo.SaveChangesAsync();
            }
        }


        public async Task SaveChangesAsync()
        {
            await _repo.SaveChangesAsync();
        }
        #endregion

        #region Response-based Methods
        public async Task<Response<string>> AddSubject(AddCourseDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return _handler.BadRequest<string>("Please enter a valid subject name");
            }

            try
            {
                var existingSubject = await _repo.GetSubjectByNameAsync(request.Name);
                if (existingSubject != null)
                {
                    return _handler.BadRequest<string>("Subject with this name already exists");
                }

                await AddSubjectAsync(request);
                return _handler.Success<string>("Subject added successfully");
            }
            catch (Exception ex)
            {
                return _handler.UnprocessableEntity<string>($"Error adding subject: {ex.Message}");
            }
        }

        public async Task<Response<string>> RemoveSubject(int subjectId)
        {
            if (subjectId <= 0)
            {
                return _handler.BadRequest<string>("Please provide a valid subject ID");
            }

            try
            {
                var subject = await _repo.GetSubjectByIdAsync(subjectId);
                if (subject == null)
                {
                    return _handler.NotFound<string>("Subject not found");
                }

                _repo.RemoveSubject(subject);
                await _repo.SaveChangesAsync();
                return _handler.Success<string>("Subject removed successfully");
            }
            catch (Exception ex)
            {
                return _handler.UnprocessableEntity<string>($"Error removing subject: {ex.Message}");
            }
        }


        public async Task<Response<string>> UpdateSubject(UpdateSubjectDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return _handler.BadRequest<string>("Please enter a valid subject name");
            }

            try
            {
                var subject = await _repo.GetSubjectByIdAsync(request.Id);
                if (subject == null)
                {
                    return _handler.NotFound<string>("Subject not found");
                }

                var existingSubject = await _repo.GetSubjectByNameAsync(request.Name);
                if (existingSubject != null && existingSubject.Id != request.Id)
                {
                    return _handler.BadRequest<string>("Another subject with this name already exists");
                }

                subject.Name = request.Name;
                _repo.UpdateSubject(subject);
                await _repo.SaveChangesAsync();
                return _handler.Success<string>("Subject updated successfully");
            }
            catch (Exception ex)
            {
                return _handler.UnprocessableEntity<string>($"Error updating subject: {ex.Message}");
            }
        }

        public async Task<Response<PagedList<ShowSubjectDTO>>> GetAllSubjects(PaginationParams pagination)
        {
            try
            {
                var query = GetSubjectsQueryable();

                var mappedQuery = query.Select(x => new ShowSubjectDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                });

                var pagedSubjects = await PagedList<ShowSubjectDTO>.CreateAsync(
                    mappedQuery,
                    pagination.PageNumber,
                    pagination.PageSize);

                return _handler.Success(pagedSubjects, "Subjects retrieved successfully");
            }
            catch (Exception ex)
            {
                return _handler.UnprocessableEntity<PagedList<ShowSubjectDTO>>(
                    $"Error retrieving subjects: {ex.Message}");
            }
        }


        public async Task<Response<string>> AssignStudentToSubject(AssignStudentDTO request)
        {
            if (request.StudentId <= 0 || request.SubjectId <= 0)
            {
                return _handler.BadRequest<string>("Please provide valid student and subject IDs");
            }

            try
            {
                var user = await _repo.GetUserByIdAsync(request.StudentId);
                if (user == null)
                {
                    return _handler.NotFound<string>("Student not found");
                }

                var subject = await _repo.GetSubjectByIdAsync(request.SubjectId);
                if (subject == null)
                {
                    return _handler.NotFound<string>("Subject not found");
                }

                var existingAssignment = await _repo.GetUserSubjectAsync(request.StudentId, request.SubjectId);
                if (existingAssignment != null)
                {
                    return _handler.BadRequest<string>("Student is already assigned to this subject");
                }

                await AddUserSubjectAsync(request);
                return _handler.Success<string>("Student assigned to subject successfully");
            }
            catch (Exception ex)
            {
                return _handler.UnprocessableEntity<string>($"Error assigning student to subject: {ex.Message}");
            }
        }

        public async Task<Response<SubjectStudentsDTO>> GetSubjectStudents(int subjectId)
        {
            if (subjectId <= 0)
            {
                return _handler.BadRequest<SubjectStudentsDTO>("Please provide a valid subject ID");
            }

            try
            {
                var result = await GetSubjectWithStudentsAsync(subjectId);
                if (result == null)
                {
                    return _handler.NotFound<SubjectStudentsDTO>("Subject not found");
                }
                return _handler.Success(result, "Subject students retrieved successfully");
            }
            catch (Exception ex)
            {
                return _handler.UnprocessableEntity<SubjectStudentsDTO>(
                    $"Error retrieving subject students: {ex.Message}");
            }
        }

        public async Task<Response<string>> RemoveStudentFromSubject(RemoveStudentDTO request)
        {
            if (request.StudentId <= 0 || request.SubjectId <= 0)
            {
                return _handler.BadRequest<string>("Please provide valid student and subject IDs");
            }

            try
            {
                var userSubject = await _repo.GetUserSubjectAsync(request.StudentId, request.SubjectId);
                if (userSubject == null)
                {
                    return _handler.NotFound<string>("Student is not assigned to this subject");
                }

                RemoveUserSubject(request);
                return _handler.Success<string>("Student removed from subject successfully");
            }
            catch (Exception ex)
            {
                return _handler.UnprocessableEntity<string>($"Error removing student from subject: {ex.Message}");
            }
        }
        #endregion

        #region Private Helpers
        private ShowSubjectDTO MapToShowSubjectDTO(Subject subject)
        {
            return new ShowSubjectDTO
            {
                Id = subject.Id,
                Name = subject.Name,
                StudentCount = subject.studentSubjects?.Count ?? 0
            };
        }

        private SubjectStudentsDTO MapToSubjectStudentsDTO(Subject subject)
        {
            return new SubjectStudentsDTO
            {
                SubjectId = subject.Id,
                SubjectName = subject.Name,
                Students = subject.studentSubjects?.Select(ss => new StudentInfoDTO
                {
                    Id = ss.User.Id,
                    FullName = ss.User.FullName,
                    Email = ss.User.Email
                }).ToList() ?? new List<StudentInfoDTO>()
            };
        }



        async Task<Response<IEnumerable<Subject>>> IInstructorService.GetUserSubjectsAsync(int userId)
        {
            var res = await _repo.GetUserSubjectsAsync(userId);
            return _handler.Success(res, "Subjects retrieved successfully");
        }
        #endregion
    }
}