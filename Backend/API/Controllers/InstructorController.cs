using API.Base;
using Application.abstracts;
using Application.DTOs.User.Application.DTOs.Subject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InstructorController : ResultController
    {
        private readonly IInstructorService _instructorService;

        public InstructorController(IInstructorService instructorService)
        {
            _instructorService = instructorService;
        }

        [HttpPost("add-subject")]
        public async Task<IActionResult> AddSubject(AddCourseDTO request)
        {
            await _instructorService.AddSubjectAsync(request);
            return Ok();
        }

        [HttpDelete("remove-subject")]
        public async Task<IActionResult> RemoveSubject(RemoveStudentDTO request)
        {
            _instructorService.RemoveSubject(request);
            return NoContent();
        }

        [HttpGet("get-all-subjects")]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await _instructorService.GetAllSubjectsAsync();
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

        [HttpPut("update-subject")]
        public async Task<IActionResult> UpdateSubject(UpdateSubjectDTO request)
        {
            var res = await _instructorService.UpdateSubject(request);
            return NewResult(res);
        }

        [HttpGet("get-subject/{id}")]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            var subject = await _instructorService.GetSubjectByIdAsync(id);
            return subject == null ? NotFound() : Ok(subject);
        }

        [HttpGet("get-subject-by-name/{name}")]
        public async Task<IActionResult> GetSubjectByName(string name)
        {
            var subject = await _instructorService.GetSubjectByNameAsync(name);
            return subject == null ? NotFound() : Ok(subject);
        }

        [HttpGet("get-user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _instructorService.GetUserByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost("add-user-subject")]
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

        [HttpDelete("remove-user-subject")]

        public async Task<IActionResult> RemoveUserSubject(RemoveStudentDTO request)
        {
            _instructorService.RemoveUserSubject(request);
            return NoContent();
        }

        [HttpGet("get-user-subject")]
        public async Task<IActionResult> GetUserSubject(int userId, int subjectId)
        {
            var result = await _instructorService.GetUserSubjectAsync(userId, subjectId);
            return result == null ? NotFound() : Ok(result);
        }


    }
}