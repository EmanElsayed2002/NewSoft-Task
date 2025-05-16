using API.Base;
using Application.abstracts;
using Application.DTOs.User;
using Application.DTOs.User.Application.DTOs.Subject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ResultController
    {
        private readonly IInstructorService _instructorService;
        private readonly ILogger<UserController> _logger;

        public UserController(IInstructorService instructorService, ILogger<UserController> logger)
        {
            _instructorService = instructorService;
            _logger = logger;
        }

        [HttpPost("Instructor/add-subject")]
        public async Task<IActionResult> AddSubject(AddCourseDTO request)
        {
            await _instructorService.AddSubjectAsync(request);
            return Ok();
        }
        //used
        [HttpDelete("Instructor/remove-subject/{SubjectId}")]
        public async Task<IActionResult> RemoveSubject([FromRoute] int subjectId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var request = new RemoveStudentDTO
            {
                StudentId = int.Parse(userId),
                SubjectId = subjectId
            };
            request.StudentId = int.Parse(userId);
            _instructorService.RemoveSubject(request);
            return NoContent();
        }
        //used
        [HttpGet("get-all-subjects")]
        public async Task<IActionResult> GetAllSubjects([FromQuery] PAginatedDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }


            var subjects = await _instructorService.GetAllSubjectsAsync(int.Parse(userId), request);
            //_logger.LogError(su);
            return Ok(subjects);
        }

        [HttpGet("get-subjects-queryable")]
        public IActionResult GetSubjectsQueryable()
        {
            var query = _instructorService.GetSubjectsQueryable();
            return Ok(query);
        }

        [HttpGet("get-subject-with-students/{subjectId}")]
        public async Task<IActionResult> GetSubjectWithStudents(int subjectId)
        {
            var result = await _instructorService.GetSubjectWithStudentsAsync(subjectId);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("Instructor/update-subject")]
        public async Task<IActionResult> UpdateSubject(UpdateSubjectDTO request)
        {
            var res = await _instructorService.UpdateSubject(request);
            return NewResult(res);
        }






        //used
        [HttpPost("Student/add-user-subject")]
        [Authorize]
        public async Task<IActionResult> AddUserSubject(AssignStudentDTO request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            request.StudentId = int.Parse(userId);
            await _instructorService.AddUserSubjectAsync(request);
            return Ok();
        }

        [HttpDelete("Student/remove-student-subject/{subjectId}")]
        [Authorize]
        public async Task<IActionResult> RemoveUserSubject([FromRoute] int subjectId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var request = new RemoveStudentDTO
            {
                StudentId = int.Parse(userId),
                SubjectId = subjectId
            };
            request.StudentId = int.Parse(userId);
            await _instructorService.RemoveUserSubject(request);
            return NoContent();
        }


        [HttpGet("Students/Subjects")]
        public async Task<IActionResult> GetUserSubjects()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }


            var result = await _instructorService.GetUserSubjectsAsync(int.Parse(userId));
            return Ok(result);
        }

    }
}