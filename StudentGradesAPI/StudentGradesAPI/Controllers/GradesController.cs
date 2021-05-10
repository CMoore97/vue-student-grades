﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGradesAPI.Models;

namespace StudentGradesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly StudentGradesDBContext _context;

        public GradesController(StudentGradesDBContext context)
        {
            _context = context;
        }

        // GET: api/Grades
        [HttpGet]
        public async Task<ActionResult<object>> GetGrades()
        {
            //return await _context.Grades.ToListAsync();
            return new
            {
                Results = await _context.
                Grades
                .Select(g => new
                {
                    g.GradeId,
                    g.Score,
                    g.Letter,
                    Student = new
                    {
                        g.Student.StudentId,
                        g.Student.StudentName,
                        g.Student.Email,
                    },
                    Course = new
                    {
                        g.Course.CourseId,
                        g.Course.CourseName,
                    }
                }).ToListAsync()
            };

        }

        // GET: api/Grades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetGrade(int id)
        {
            //var grade = await _context.Grades.FindAsync(id);
            var grade = await _context.Grades.Where(g => g.GradeId == id)
                .Select(g => new
                {
                    g.GradeId,
                    g.Score,
                    g.Letter,
                    Student = new
                    {
                        g.Student.StudentId,
                        g.Student.StudentName,
                        g.Student.Email,
                    },
                    Course = new
                    {
                        g.Course.CourseId,
                        g.Course.CourseName,
                    }
                }).FirstOrDefaultAsync();

            if (grade == null)
            {
                return NotFound();
            }

            return grade;
        }

        // PUT: api/Grades/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGrade(int id, Grade grade)
        {
            if (id != grade.GradeId)
            {
                return BadRequest();
            }

            _context.Entry(grade).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GradeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Grades
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Grade>> PostGrade(Grade grade)
        {
            _context.Grades.Add(grade);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (GradeExists(grade.GradeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetGrade", new { id = grade.GradeId }, grade);
        }

        // DELETE: api/Grades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null)
            {
                return NotFound();
            }

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GradeExists(int id)
        {
            return _context.Grades.Any(e => e.GradeId == id);
        }
    }
}
