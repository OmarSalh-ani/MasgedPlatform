using masabihq8Shared;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace masabihq8Teacher
{
    public partial class PlanLevel : Page
    {
        private readonly Model1 db = new Model1();
        private readonly TheUser user = new TheUser();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
                CancelEditBtn.Visible = false;
                BindSurahDropDowns();
                BindJozzDropDowns();
                InitDates();
                BindReadyPlans();
            }
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            string levelName = LevelNameTb.Text?.Trim() ?? "";
            masabihq8Shared.PlanUnitType unitTypeEnum = (masabihq8Shared.PlanUnitType)byte.Parse(UnitTypeDdl.SelectedValue);
            int quantity = 0;
            int.TryParse(QuantityTb.Text, out quantity);

            if (string.IsNullOrEmpty(levelName) || quantity <= 0)
                return;

            int editId = 0;
            int.TryParse(EditIdHidden.Value, out editId);

            if (editId > 0)
            {
                var entity = db.PlanLevels.FirstOrDefault(x => x.Id == editId && x.CreatedByTeacherId == user.Id());
                if (entity != null)
                {
                    entity.LevelName = levelName;
                    entity.UnitType = (byte)unitTypeEnum;
                    entity.Quantity = quantity;
                    db.SaveChanges();
                }
            }
            else
            {
                var entity = new masabihq8Shared.PlanLevel
                {
                    LevelName = levelName,
                    UnitType = (byte)unitTypeEnum,
                    Quantity = quantity,
                    CreatedAt = KuwaitTime.Now,
                    CreatedByTeacherId = user.Id()
                };
                db.PlanLevels.Add(entity);
                db.SaveChanges();
            }

            ClearForm();
            BindGrid();
        }

        protected void CancelEditBtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void gvLevels_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = 0;
            int.TryParse(e.CommandArgument.ToString(), out id);
            if (id <= 0) return;

            if (e.CommandName == "editLevel")
            {
                var entity = db.PlanLevels.FirstOrDefault(x => x.Id == id && x.CreatedByTeacherId == user.Id());
                if (entity != null)
                {
                    EditIdHidden.Value = entity.Id.ToString();
                    LevelNameTb.Text = entity.LevelName;
                    UnitTypeDdl.SelectedValue = entity.UnitType.ToString();
                    QuantityTb.Text = entity.Quantity.ToString();
                    CancelEditBtn.Visible = true;
                }
            }
            else if (e.CommandName == "deleteLevel")
            {
                var us = user.Id();
                var entity = db.PlanLevels.FirstOrDefault(x => x.Id == id && x.CreatedByTeacherId == us);
                if (entity != null)
                {
                    db.PlanLevels.Remove(entity);
                    db.SaveChanges();
                    ClearForm();
                    BindGrid();
                }
            }
        }

        private void BindGrid()
        {
            int teacherId = user.Id();
            var data = db.PlanLevels
                .Where(x => x.CreatedByTeacherId == null || x.CreatedByTeacherId == teacherId)
                .OrderByDescending(x => x.Id)
                .ToList();
            gvLevels.DataSource = data;
            gvLevels.DataBind();
        }

        private void BindSurahDropDowns()
        {
            var surahs = db.QuranSurahs
                .OrderBy(x => x.SortOrder ?? x.Id)
                .Select(x => new { x.Id, Name = "سورة " + x.NameAr })
                .ToList();
            FromSurahDropDown.DataSource = surahs;
            FromSurahDropDown.DataTextField = "Name";
            FromSurahDropDown.DataValueField = "Id";
            FromSurahDropDown.DataBind();
            ToSurahDropDown.DataSource = surahs;
            ToSurahDropDown.DataTextField = "Name";
            ToSurahDropDown.DataValueField = "Id";
            ToSurahDropDown.DataBind();
        }

        private void BindJozzDropDowns()
        {
            var jozzList = Enumerable.Range(1, 30).Select(i => new { Id = i, Name = "جزء " + i }).ToList();
            FromJozzDdl.DataSource = jozzList;
            FromJozzDdl.DataTextField = "Name";
            FromJozzDdl.DataValueField = "Id";
            FromJozzDdl.DataBind();
            ToJozzDdl.DataSource = jozzList;
            ToJozzDdl.DataTextField = "Name";
            ToJozzDdl.DataValueField = "Id";
            ToJozzDdl.DataBind();
        }

        private void InitDates()
        {
            if (string.IsNullOrEmpty(FromDateTb.Text)) FromDateTb.Text = KuwaitTime.Today.ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(ToDateTb.Text)) ToDateTb.Text = KuwaitTime.Today.ToString("yyyy-MM-dd");
        }

        protected void SaveReadyPlanBtn_Click(object sender, EventArgs e)
        {
            int fromSurahId = int.Parse(FromSurahDropDown.SelectedValue);
            int toSurahId = int.Parse(ToSurahDropDown.SelectedValue);

            int? fromAyah = null;
            if (int.TryParse(FromAyahTb.Text, out int fa)) fromAyah = fa;
            int? toAyah = null;
            if (int.TryParse(ToAyahTb.Text, out int ta)) toAyah = ta;

            int? fromJozz = null;
            if (int.TryParse(FromJozzDdl.SelectedValue, out int fj)) fromJozz = fj;
            int? toJozz = null;
            if (int.TryParse(ToJozzDdl.SelectedValue, out int tj)) toJozz = tj;

            DateTime fromDate = KuwaitTime.Today;
            DateTime toDate = KuwaitTime.Today;
            DateTime.TryParse(FromDateTb.Text, out fromDate);
            DateTime.TryParse(ToDateTb.Text, out toDate);
            if (toDate < fromDate) toDate = fromDate;

            int rpEditId = 0;
            int.TryParse(ReadyPlanEditIdHidden.Value, out rpEditId);

            if (rpEditId > 0)
            {
                var existing = db.ReadyPlans.FirstOrDefault(x => x.Id == rpEditId && x.CreatedByTeacherId == user.Id());
                if (existing != null)
                {
                    existing.FromSurahId = fromSurahId;
                    existing.ToSurahId = toSurahId;
                    existing.FromAyah = fromAyah;
                    existing.ToAyah = toAyah;
                    existing.FromJozz = fromJozz;
                    existing.ToJozz = toJozz;
                    existing.FromDate = fromDate.Date;
                    existing.ToDate = toDate.Date;
                    db.SaveChanges();
                    ReadyPlanEditIdHidden.Value = "";
                    BindReadyPlans();
                    return;
                }
            }

            string levelName = LevelNameTb.Text?.Trim() ?? "";
            int planLevelId = 0;
            var By = user.Id();
            var level = db.PlanLevels.FirstOrDefault(pl => pl.LevelName == levelName && (pl.CreatedByTeacherId == null || pl.CreatedByTeacherId == By));
            if (level != null) planLevelId = level.Id;

            if (planLevelId == 0 && !string.IsNullOrEmpty(levelName))
            {
                var unitTypeEnum = (masabihq8Shared.PlanUnitType)byte.Parse(UnitTypeDdl.SelectedValue);
                int qty = 0; int.TryParse(QuantityTb.Text, out qty);
                if (qty <= 0) qty = 1;
                var newLevel = new masabihq8Shared.PlanLevel
                {
                    LevelName = levelName,
                    UnitType = (byte)unitTypeEnum,
                    Quantity = qty,
                    CreatedAt = KuwaitTime.Now,
                    CreatedByTeacherId = user.Id()
                };
                db.PlanLevels.Add(newLevel);
                db.SaveChanges();
                planLevelId = newLevel.Id;
            }

            var rp = new ReadyPlan
            {
                PlanLevelId = planLevelId,
                FromSurahId = fromSurahId,
                ToSurahId = toSurahId,
                FromAyah = fromAyah,
                ToAyah = toAyah,
                FromJozz = fromJozz,
                ToJozz = toJozz,
                FromDate = fromDate.Date,
                ToDate = toDate.Date,
                CreatedAt = KuwaitTime.Now,
                CreatedByTeacherId = user.Id()
            };
            db.ReadyPlans.Add(rp);
            db.SaveChanges();
            BindReadyPlans();
        }

        private void BindReadyPlans()
        {
            int teacherId = user.Id();
            var surahNames = db.QuranSurahs.ToDictionary(s => s.Id, s => "سورة " + s.NameAr);
            var data = db.ReadyPlans
                .Where(x => x.CreatedByTeacherId == null || x.CreatedByTeacherId == teacherId)
                .OrderByDescending(x => x.Id)
                .ToList()
                .Select(x =>
                {
                    var level = db.PlanLevels.FirstOrDefault(pl => pl.Id == x.PlanLevelId);
                    string fromDisp = "", toDisp = "";
                    if (level != null && (masabihq8Shared.PlanUnitType)level.UnitType == masabihq8Shared.PlanUnitType.Jozz)
                    {
                        fromDisp = "جزء " + x.FromJozz;
                        toDisp = "جزء " + x.ToJozz;
                    }
                    else
                    {
                        string s1 = surahNames.ContainsKey(x.FromSurahId) ? surahNames[x.FromSurahId] : x.FromSurahId.ToString();
                        string s2 = surahNames.ContainsKey(x.ToSurahId) ? surahNames[x.ToSurahId] : x.ToSurahId.ToString();
                        fromDisp = s1;
                        toDisp = s2;
                    }

                    return new
                    {
                        x.Id,
                        LevelName = level != null ? level.LevelName : "—",
                        FromSurahName = fromDisp,
                        ToSurahName = toDisp,
                        x.FromDate,
                        x.ToDate,
                        x.CreatedAt,
                        x.CreatedByTeacherId
                    };
                })
                .ToList();
            gvReadyPlans.DataSource = data;
            gvReadyPlans.DataBind();
        }

        protected void gvReadyPlans_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = 0;
            int.TryParse(e.CommandArgument.ToString(), out id);
            if (id <= 0) return;
            if (e.CommandName == "editReadyPlan")
            {
                var rp = db.ReadyPlans.FirstOrDefault(x => x.Id == id && x.CreatedByTeacherId == user.Id());
                if (rp != null)
                {
                    ReadyPlanEditIdHidden.Value = rp.Id.ToString();
                    var level = db.PlanLevels.FirstOrDefault(pl => pl.Id == rp.PlanLevelId);
                    if (level != null)
                    {
                        LevelNameTb.Text = level.LevelName;
                        UnitTypeDdl.SelectedValue = ((byte)level.UnitType).ToString();
                        QuantityTb.Text = level.Quantity.ToString();
                    }
                    FromSurahDropDown.SelectedValue = rp.FromSurahId.ToString();
                    ToSurahDropDown.SelectedValue = rp.ToSurahId.ToString();
                    FromAyahTb.Text = rp.FromAyah?.ToString() ?? "";
                    ToAyahTb.Text = rp.ToAyah?.ToString() ?? "";
                    if (rp.FromJozz.HasValue) FromJozzDdl.SelectedValue = rp.FromJozz.Value.ToString();
                    if (rp.ToJozz.HasValue) ToJozzDdl.SelectedValue = rp.ToJozz.Value.ToString();
                    FromDateTb.Text = rp.FromDate.ToString("yyyy-MM-dd");
                    ToDateTb.Text = rp.ToDate.ToString("yyyy-MM-dd");
                }
            }
            else if (e.CommandName == "deleteReadyPlan")
            {
                var rp = db.ReadyPlans.FirstOrDefault(x => x.Id == id && x.CreatedByTeacherId == user.Id());
                if (rp != null)
                {
                    db.ReadyPlans.Remove(rp);
                    db.SaveChanges();
                    BindReadyPlans();
                }
            }
        }

        protected string GetUnitDisplay(object unitTypeObj)
        {
            int numeric = 0;
            if (unitTypeObj != null) int.TryParse(unitTypeObj.ToString(), out numeric);
            var unitType = (masabihq8Shared.PlanUnitType)numeric;
            switch (unitType)
            {
                case masabihq8Shared.PlanUnitType.Page: return "صفحة";
                case masabihq8Shared.PlanUnitType.QuarterPage: return "ربع";
                case masabihq8Shared.PlanUnitType.Jozz: return "جزء";
                case masabihq8Shared.PlanUnitType.Line: return "سطر";
                default: return "";
            }
        }

        private void ClearForm()
        {
            EditIdHidden.Value = "";
            LevelNameTb.Text = "";
            UnitTypeDdl.SelectedIndex = 0;
            QuantityTb.Text = "";
            FromAyahTb.Text = "";
            ToAyahTb.Text = "";
            FromJozzDdl.SelectedIndex = 0;
            ToJozzDdl.SelectedIndex = 0;
            FromDateTb.Text = KuwaitTime.Today.ToString("yyyy-MM-dd");
            ToDateTb.Text = KuwaitTime.Today.ToString("yyyy-MM-dd");
            CancelEditBtn.Visible = false;
            ReadyPlanEditIdHidden.Value = "";
        }
    }
}
