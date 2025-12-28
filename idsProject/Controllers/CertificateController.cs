using Ids.Data;
using Ids.Models;
using idsProject.Dtos.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ids.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificateController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CertificateController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Certificate
        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var certificates = await _context.Certificates
                .Select(c => new CertificateResponseDto
                {
                    Id = c.Id,
                    DownloadUrl = c.DownloadUrl,
                    CourseId = c.CourseId,
                    UserId = c.UserId,
                    GeneratedAt = c.GeneratedAt
                })
                .ToListAsync();

            return Ok(certificates);
        }

        // GET: api/Certificate/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cert = await _context.Certificates.FindAsync(id);
            if (cert == null) return NotFound();

            var response = new CertificateResponseDto
            {
                Id = cert.Id,
                DownloadUrl = cert.DownloadUrl,
                CourseId = cert.CourseId,
                UserId = cert.UserId,
                GeneratedAt = cert.GeneratedAt
            };

            return Ok(response);
        }

        // POST: api/Certificate
        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCertificateDto dto)
        {
            var cert = new Certificate
            {
                DownloadUrl = dto.DownloadUrl,
                CourseId = dto.CourseId,
                UserId = dto.UserId
            };

            _context.Certificates.Add(cert);
            await _context.SaveChangesAsync();

            var response = new CertificateResponseDto
            {
                Id = cert.Id,
                DownloadUrl = cert.DownloadUrl,
                CourseId = cert.CourseId,
                UserId = cert.UserId,
                GeneratedAt = cert.GeneratedAt
            };

            return Ok(response);
        }

        // DELETE: api/Certificate/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cert = await _context.Certificates.FindAsync(id);
            if (cert == null) return NotFound();

            _context.Certificates.Remove(cert);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
