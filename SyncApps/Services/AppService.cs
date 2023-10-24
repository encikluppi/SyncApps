using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SyncApps.Models;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using SyncApps.Models.ViewModels;
using SyncApps.Data;
using Microsoft.EntityFrameworkCore;

namespace SyncApps.Services
{
    public class AppService
    {
        public string BASEURL = "http://test-demo.aemenersol.com/api/";
        private readonly AppDbContext _context;

        public AppService(AppDbContext appContext)
        {
            _context = appContext;
        }
        public async Task<string> Login()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    Dictionary<string, string> jsonValues = new Dictionary<string, string>();
                    jsonValues.Add("username", "user@aemenersol.com");
                    jsonValues.Add("password", "Test@123");

                    var content = new StringContent(JsonConvert.SerializeObject(jsonValues), Encoding.UTF8, "application/json");

                    using (var response = await client.PostAsync($"{BASEURL}account/login", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var token = JsonConvert.DeserializeObject<string>(apiResponse);

                        return token;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public async Task<bool> SyncTask(string token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    using (var response = await client.GetAsync($"{BASEURL}PlatformWell/GetPlatformWellActual"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            var jsonObj = JsonConvert.DeserializeObject<List<PlatformVM>>(result);

                            bool apistatus = await SyncData(jsonObj);

                            return apistatus = true ? true : false;

                        }

                        return false;


                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SyncData(List<PlatformVM> resultData)
        {
            try
            {

                foreach (PlatformVM item in resultData)
                {
                    //check platform id exist
                    var obj_sam2 = await _context.Platform.FindAsync(item.Id);
                    var PlatformId = item.Id;

                    //if platform not exist yet thne create
                    if (obj_sam2 == null)
                    {
                        Platform obj_sam = new Platform();

                        obj_sam.Id = item.Id;
                        obj_sam.UniqueName = item.UniqueName;
                        obj_sam.Latitude = item.Latitude;
                        obj_sam.Longitude = item.Longitude;

                        if (item.CreatedAt.HasValue)
                            obj_sam.CreatedAt = (DateTime)item.CreatedAt;
                        else
                            obj_sam.CreatedAt = DateTime.Now;

                        if (item.UpdatedAt.HasValue)
                            obj_sam.UpdatedAt = (DateTime)item.UpdatedAt;
                        else
                            obj_sam.UpdatedAt = null;

                        await _context.Platform.AddAsync(obj_sam);
                        //bool hasChanges = _context.ChangeTracker.HasChanges();
                        await _context.SaveChangesAsync();

                        PlatformId = obj_sam.Id;

                    }
                    else//update platform
                    {
                        obj_sam2.UniqueName = item.UniqueName;
                        obj_sam2.Latitude = item.Latitude;
                        obj_sam2.Longitude = item.Longitude;
                        if(item.CreatedAt.HasValue)
                            obj_sam2.CreatedAt = (DateTime)item.CreatedAt;
                        

                        if (item.UpdatedAt.HasValue)
                            obj_sam2.UpdatedAt = (DateTime)item.UpdatedAt;
                        else
                            obj_sam2.UpdatedAt = (DateTime)obj_sam2.UpdatedAt;


                        await _context.SaveChangesAsync();

                    }

                    //Well
                    if(item.Well != null)
                    {
                        foreach (WellVM well_item in item.Well)
                        {
                            //check if well id exist
                            var checkWell= await _context.Well.FindAsync(well_item.Id);

                            //if well not exist then create
                            if (checkWell == null)
                            {
                            
                                Well obj_sam3 = new Well();

                                obj_sam3.Id = well_item.Id;
                                obj_sam3.PlatformId = PlatformId;
                                obj_sam3.UniqueName = well_item.UniqueName;
                                obj_sam3.Latitude = well_item.Latitude;
                                obj_sam3.Longitude = well_item.Longitude;

                                if (item.CreatedAt.HasValue)
                                    obj_sam3.CreatedAt = (DateTime)well_item.CreatedAt;
                                else
                                    obj_sam3.CreatedAt = DateTime.Now;

                                if (item.UpdatedAt.HasValue)
                                    obj_sam3.UpdatedAt = (DateTime)well_item.UpdatedAt;
                                else
                                    obj_sam3.UpdatedAt = DateTime.Now;

                                await _context.Well.AddAsync(obj_sam3);
                                await _context.SaveChangesAsync();
                            }
                            else//update
                            {
                                checkWell.PlatformId = PlatformId;
                                checkWell.UniqueName = well_item.UniqueName;
                                checkWell.Latitude = well_item.Latitude;
                                checkWell.Longitude = well_item.Longitude;
                                if (item.CreatedAt.HasValue)
                                    checkWell.CreatedAt = (DateTime)well_item.CreatedAt;
                          

                                if (well_item.UpdatedAt.HasValue)
                                    checkWell.UpdatedAt = (DateTime)well_item.UpdatedAt;
                                else
                                    checkWell.UpdatedAt = (DateTime)checkWell.UpdatedAt;


                                await _context.SaveChangesAsync();
                            }

                        }
                    }
                    
                }

                return true;

            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}
