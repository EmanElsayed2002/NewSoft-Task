namespace Application.DTOs.User
{


    namespace Application.DTOs.Subject
    {
        public class ShowSubjectDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int StudentCount { get; set; }
        }

        public class AddCourseDTO
        {
            public string Name { get; set; }
        }

        public class UpdateSubjectDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class RemoveStudentDTO
        {
            public int StudentId { get; set; }
            public int SubjectId { get; set; }
        }

        public class AssignStudentDTO
        {
            public int StudentId { get; set; }
            public int SubjectId { get; set; }
        }

        public class SubjectStudentsDTO
        {
            public int SubjectId { get; set; }
            public string SubjectName { get; set; }
            public List<StudentInfoDTO> Students { get; set; } = new List<StudentInfoDTO>();
        }

        public class StudentInfoDTO
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
        }
        public class UserInfoDTO
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string? Address { get; set; }
            public string? Phone { get; set; }
            public int Age { get; set; }
        }
    }


}
