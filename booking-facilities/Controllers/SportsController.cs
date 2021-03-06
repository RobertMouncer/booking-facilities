﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using booking_facilities.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;
using booking_facilities.Repositories;
using AberFitnessAuditLogger;

namespace booking_facilities.Controllers
{
    [Authorize(AuthenticationSchemes = "oidc", Policy = "Administrator")]
    public class SportsController : Controller
    {
        private readonly ISportRepository sportRepository;
        private readonly IAuditLogger auditLogger;

        public SportsController(ISportRepository sportRepository, IAuditLogger auditLogger)
        {
            this.sportRepository = sportRepository;
            this.auditLogger = auditLogger;
        }

        // GET: Sports
        public async Task<IActionResult> Index(int? page)
        {
            await auditLogger.log(GetUserId(), "Accessed Sport Index");
            var sportsQ = sportRepository.GetAllAsync().OrderBy(s => s.SportName);
            var sportsList = await sportsQ.ToListAsync();

            var pageNumber = page ?? 1; 
            var sportsPerPage = 10;

            var onePageOfSports = sportsList.ToPagedList(pageNumber, sportsPerPage); 

            ViewBag.onePageOfSports = onePageOfSports;
            return View();
        }

        // GET: Sports/Create
        public async Task<IActionResult> Create()
        {
            await auditLogger.log(GetUserId(), "Accessed Create Sport");
            return View();
        }

        // POST: Sports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SportId,SportName")] Sport sport)
        {
            if (sportRepository.DoesSportExist(sport.SportName))
            {
                ModelState.AddModelError("SportName","Sport already exists. Please enter another sport.");
            }

            if (ModelState.IsValid)
            {
                sport = await sportRepository.AddAsync(sport);
                await auditLogger.log(GetUserId(), $"Created Sport {sport.SportName}");
                return RedirectToAction(nameof(Index));
            }
            return View(sport);
        }

        // GET: Sports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            await auditLogger.log(GetUserId(), $"Accessed Edit Sport {id}");
            var sport = await sportRepository.GetByIdAsync(id.Value);
            if (sport == null)
            {
                return NotFound();
            }
            return View(sport);
        }

        // POST: Sports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SportId,SportName")] Sport sport)
        {
            if (id != sport.SportId)
            {
                return NotFound();
            }


            if (sportRepository.DoesSportExist(sport.SportName))
            {
                ModelState.AddModelError("SportName", "Sport already exists. Please enter another sport.");
            }
            else if(ModelState.IsValid)
            {
                try
                {
                    await sportRepository.UpdateAsync(sport);
                    await auditLogger.log(GetUserId(), $"Edited Sport {id}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!sportRepository.SportIdExists(sport.SportId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sport);
        }

        // GET: Sports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            await auditLogger.log(GetUserId(), $"Accessed Delete Sport {id}");
            var sport = await sportRepository.GetByIdAsync(id.Value);

            if (sport == null)
            {
                return NotFound();
            }

            return View(sport);
        }

        // POST: Sports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sport = await sportRepository.GetByIdAsync(id);
            await sportRepository.DeleteAsync(sport);
            await auditLogger.log(GetUserId(), $"Deleted Sport {id}");
            return RedirectToAction(nameof(Index));
        }
        public string GetUserId()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
        }
    }
}
