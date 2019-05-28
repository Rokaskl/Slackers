using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdditionalDatasController : ControllerBase
    {
        private readonly DataContext _context;

        public AdditionalDatasController(DataContext context)
        {
            _context = context;
        }

        // GET: AdditionalDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdditionalData>>> GetAdditionalDatas()
        {
            return await _context.AdditionalDatas.ToListAsync();
        }

        // GET: AdditionalDatas/5
        [HttpGet("{id}/{isUser}")]
        public async Task<ActionResult<AdditionalData>> GetAdditionalData(int id, bool isUser)
        {
            var additionalData = await _context.AdditionalDatas.FirstAsync(x=>x.Ownerid==id&&x.IsUser==isUser);

            if (additionalData == null)
            {
                return NotFound();
            }

            return additionalData;
        }

        // PUT: AdditionalDatas/5 --NENAUDOTI
        [HttpPut("{id}/{isUser}")]
        public async Task<IActionResult> PutAdditionalData(int id,bool isUser, AdditionalData additionalData)
        {
            if (id != additionalData.Ownerid||isUser!=additionalData.IsUser)
            {
                return BadRequest();
            }

            _context.Entry(additionalData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdditionalDataExists(id,isUser))
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

        // POST: AdditionalDatas naudoti šitą
        [HttpPost]
        public async Task<ActionResult<AdditionalData>> PostAdditionalData(AdditionalData additionalData)
        {
            try
            {
                if (AdditionalDataExists(additionalData.Ownerid,additionalData.IsUser))
                    {
                    if (additionalData.PhotoBytes==null)
                    {
                        additionalData.PhotoBytes = _context.AdditionalDatas.FirstOrDefault(x=>x.IsUser==additionalData.IsUser&&x.Ownerid==additionalData.Ownerid).PhotoBytes;
                    }
                    await DeleteAdditionalData(additionalData.Ownerid,additionalData.IsUser);
                    }
               
                    _context.AdditionalDatas.Add(additionalData); 
                    

                _context.SaveChanges();

                return Ok(_context.AdditionalDatas.First(x=>x.Ownerid==additionalData.Ownerid&&x.IsUser==additionalData.IsUser));
            }
            catch (Exception ex)
            {                
                return BadRequest();
                throw new AppException(ex.Message);
            }

        }

        // DELETE: AdditionalDatas/5
        [HttpDelete("{id}/{isUser}")]
        public async Task<ActionResult<AdditionalData>> DeleteAdditionalData(int id,bool isUser)
        {
            var additionalData = await _context.AdditionalDatas.FirstAsync(x=>x.Ownerid==id&&x.IsUser==isUser);
            if (additionalData == null)
            {
                return NotFound();
            }

            _context.AdditionalDatas.Remove(additionalData);
            await _context.SaveChangesAsync();

            return additionalData;
        }

        private bool AdditionalDataExists(int id,bool isUser)
        {
            return _context.AdditionalDatas.Any(e => e.Ownerid == id&&e.IsUser==isUser);
        }
    }
}
