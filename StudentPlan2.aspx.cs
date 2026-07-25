using masabihq8Shared;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace masabihq8Teacher
{


    public partial class StudentPlan2 : System.Web.UI.Page
    {
        private class PlanRowVM
        {
            public string Key { get; set; }
            public string PlanType { get; set; }
            public string MemorizationLevel { get; set; }
            public int SurahId { get; set; }
            public string SurahName { get; set; }
            public int FromAyahNumber { get; set; }
            public int ToAyahNumber { get; set; }
            public DateTime PlanDate { get; set; }
            public string PlanDateFormatted { get; set; }
            public string LatestStatus { get; set; }
            public DateTime? MemorizeDate { get; set; }
            public DateTime? ReviseDate { get; set; }
        }

        private class CalendarSurahVM
        {
            public string SurahName { get; set; }
            public int SurahId { get; set; }
            public string PlanType { get; set; }
        }

        private class CalendarDayVM
        {
            public DateTime Date { get; set; }
            public bool IsCircleDay { get; set; }
            public List<CalendarSurahVM> Items { get; set; } = new List<CalendarSurahVM>();
            public string DateDisplay => Date.ToString("yyyy-MM-dd");
            public string DayNameAr
            {
                get
                {
                    var names = new[] { "الأحد", "الاثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت" };
                    return names[(int)Date.DayOfWeek];
                }
            }
            public string ItemsHtml { get; set; }
        }

        public const string StatusPass = "تم";
        public const string StatusFail = "لم يتم";
        public const string StatusPending = "قيد الانتظار";
        public const string StatusRetake = "اعادة تسميع";
        public const string StatusPendingTathbit = "قيد الانتظار في التثبيت";

        private static string GetStatusDisplayLabel(string rowKey, string status)
        {
            if (string.IsNullOrEmpty(status)) return "";
            if (rowKey.StartsWith("memorizing_")) return status == StatusPass ? "تم الحفظ" : status == StatusFail ? "لم يتم الحفظ" : status == StatusRetake ? "اعادة تسميع" : status == StatusPendingTathbit ? "قيد الانتظار" : status;
            if (rowKey.StartsWith("revise_")) return status == StatusPass ? "تم المراجعة" : status == StatusFail ? "لم يتم المراجعة" : status == StatusRetake ? "اعادة مراجعة" : status;
            return status == StatusPass ? "تم التثبيت" : status == StatusFail ? "لم يتم التثبيت" : status;
        }

        private readonly Model1 db = new Model1();
        private readonly TheUser user = new TheUser();

        /// <summary>Returns true when the logged-in teacher's circle matches the student's circle (Teacher Panel scope).</summary>
        private static bool CurrentTeacherOwnsStudent(Model1 dbCtx, int studentId)
        {
            var u = new TheUser();
            int circleId = u.CircleId();
            if (circleId <= 0) return false;
            return dbCtx.RegisterForms.Any(r => r.Id == studentId && r.QuranCircleId == circleId);
        }

        private int PlanRowCount
        {
            get
            {
                if (ViewState["PlanRowCount"] == null)
                    return 1;
                return (int)ViewState["PlanRowCount"];
            }
            set { ViewState["PlanRowCount"] = value; }
        }

        private int PlanRowCountRevise
        {
            get
            {
                if (ViewState["PlanRowCountRevise"] == null)
                    return 1;
                return (int)ViewState["PlanRowCountRevise"];
            }
            set { ViewState["PlanRowCountRevise"] = value; }
        }

        private int? ViewStudentId
        {
            get
            {
                string s = Request.QueryString["studentId"];
                if (string.IsNullOrEmpty(s)) return null;
                return int.TryParse(s, out int id) ? id : (int?)null;
            }
        }

        private int? ViewPlanId
        {
            get
            {
                string s = Request.QueryString["planId"];
                if (!string.IsNullOrEmpty(s) && int.TryParse(s, out int id))
                    return id;
                if (HiddenViewPlanId != null && !string.IsNullOrEmpty(HiddenViewPlanId.Value) && int.TryParse(HiddenViewPlanId.Value, out int hidId))
                    return hidId;
                return null;
            }
        }

        private bool IsNewPlanMode => !string.IsNullOrEmpty(Request.QueryString["newPlan"]) && Request.QueryString["newPlan"] == "1";

        private bool EditMode
        {
            get { return ViewState["PlanEditMode"] as bool? ?? false; }
            set { ViewState["PlanEditMode"] = value; }
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            if (IsPostBack)
            {
                LoadStudents();
                LoadSurahs();
                BindPlanRows();
                BindPlanRowsRevise();
            }
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Charset = "utf-8";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStudents();
                LoadSurahs();

                string editKey = Request.QueryString["edit"];
                if (!string.IsNullOrEmpty(editKey))
                {
                    ViewState["EditPlanKey"] = editKey;
                    PlanRowCount = 1;
                }
                else
                {
                 

                    // Trigger initial calculation set via ScriptManager or handle in JS
                    ScriptManager.RegisterStartupScript(this, GetType(), "initCircleDays", "if(typeof updateCircleDaysCount === 'function') updateCircleDaysCount();", true);
                }

                BindPlanRows();
                BindPlanRowsRevise();
            }

            // View mode and plan binding: run on every load (including postback) when viewing a student
            int? studentId = ViewStudentId;
            if (studentId.HasValue)
            {
                int circleIdGate = user.CircleId();
                if (circleIdGate <= 0 || !db.RegisterForms.Any(r => r.Id == studentId.Value && r.QuranCircleId == circleIdGate))
                {
                    Response.Redirect("StudentPlan2.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                PanelViewMode.Visible = true;
                PanelStudentsSelect.Visible = false;
                if (HiddenViewStudentId != null) HiddenViewStudentId.Value = ViewStudentId.Value.ToString();

                


                if (IsNewPlanMode)
                {
                    if (PanelPlanView != null) PanelPlanView.Visible = false;
                    if (PanelAssessmentLog != null) PanelAssessmentLog.Visible = false;
                    
                    if (TabSingleCtrl != null) { TabSingleCtrl.ShowActions = true; }
                    if (TabReviseCtrl != null) { TabReviseCtrl.ShowActions = true; }
                    if (DeletePlanLinkBtn != null) DeletePlanLinkBtn.Visible = false;
                }
                else
                {
                    if (PanelPlanView != null) PanelPlanView.Visible = true;
                    if (PanelAssessmentLog != null) PanelAssessmentLog.Visible = true;

                    int? planId = ViewPlanId;
                    if (DeletePlanLinkBtn != null && planId.HasValue) DeletePlanLinkBtn.Visible = true;
                    string editKey = Request.QueryString["edit"];
                    if (planId == null && !string.IsNullOrEmpty(editKey))
                    {
                        if (editKey.StartsWith("memorizing_") && int.TryParse(editKey.Substring("memorizing_".Length), out int memId))
                        {
                            var ent = db.StudentPlanMemorizings.FirstOrDefault(x => x.Id == memId && x.StudentId == studentId.Value);
                            if (ent != null) planId = ent.PlanId;
                        }
                        else if (editKey.StartsWith("revise_") && int.TryParse(editKey.Substring("revise_".Length), out int revId))
                        {
                            var ent = db.StudentPlanRevises.FirstOrDefault(x => x.Id == revId && x.StudentId == studentId.Value);
                            if (ent != null) planId = ent.PlanId;
                        }
                    }
                    var plans = db.StudentPlans.Where(p => p.StudentId == studentId.Value && !p.IsArchived).OrderBy(p => p.PlanFromDate).ToList();
                    if (planId == null && plans.Count > 0)
                    {
                        var today = KuwaitTime.Today;
                        var currentPlan = plans.FirstOrDefault(p => today >= p.PlanFromDate && today <= p.PlanToDate) ?? plans.First();
                        string qs = "studentId=" + studentId.Value + "&planId=" + currentPlan.Id;
                        if (!string.IsNullOrEmpty(editKey)) qs += "&edit=" + HttpUtility.UrlEncode(editKey);
                        Response.Redirect("StudentPlan2.aspx?" + qs, false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                    if (planId == null && plans.Count == 0)
                    {
                        Response.Redirect("StudentPlan2.aspx?studentId=" + studentId.Value + "&newPlan=1", false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                    if (HiddenViewPlanId != null && planId.HasValue) HiddenViewPlanId.Value = planId.Value.ToString();

                    if (EditModeCheckBox != null)
                        EditMode = EditModeCheckBox.Checked;
                    var student = db.RegisterForms.Where(x => x.Id == studentId.Value).Select(x => new { x.Id, Name = x.FullName ?? x.StudentName }).FirstOrDefault();
                    if (student != null)
                    {
                        if (LiteralHeaderTitle != null) LiteralHeaderTitle.Text = "خطة الطالب - " + student.Name;
                    }
                    LoadPlanListForStudent(studentId.Value);
                    if (planId.HasValue)
                    {
                        LoadPlansForStudent(studentId.Value, planId.Value);
                        if (!IsPostBack)
                            LoadPlanHeaderFromDatabase(planId.Value);
                    }
                    if (TabSingleCtrl != null) TabSingleCtrl.ShowActions = true;
                    if (TabReviseCtrl != null) TabReviseCtrl.ShowActions = true;
                    if (TabTathbitCtrl != null) TabTathbitCtrl.ShowActions = false;
                }
            }
            else
            {
                PanelViewMode.Visible = false;
                PanelStudentsSelect.Visible = true;
                if (PanelAssessmentLog != null) PanelAssessmentLog.Visible = false;
                if (TabSingleCtrl != null) TabSingleCtrl.ShowActions = true;
                if (TabReviseCtrl != null) TabReviseCtrl.ShowActions = true;
                if (TabTathbitCtrl != null) TabTathbitCtrl.ShowActions = false;
                if (DeletePlanLinkBtn != null) DeletePlanLinkBtn.Visible = false;
            }
        }

        protected void DeletePlanLinkBtn_Click(object sender, EventArgs e)
        {
            int? planId = ViewPlanId;
            int? studentId = ViewStudentId;

            if (planId.HasValue && studentId.HasValue)
            {
                if (!CurrentTeacherOwnsStudent(db, studentId.Value))
                    return;

                var plan = db.StudentPlans.FirstOrDefault(p => p.Id == planId.Value && p.StudentId == studentId.Value);
                if (plan != null)
                {
                    // Archive instead of delete: keep plan and all data, mark as archived
                    plan.IsArchived = true;
                    db.SaveChanges();

                    // Redirect to the student page (it will handle redirecting to another plan or new plan mode)
                    Response.Redirect("StudentPlan2.aspx?studentId=" + studentId.Value, false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
        }

       

       

        protected void PlanNameDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            int? studentId = ViewStudentId;
            if (!studentId.HasValue || PlanNameDropDown == null || string.IsNullOrEmpty(PlanNameDropDown.SelectedValue)) return;
            if (int.TryParse(PlanNameDropDown.SelectedValue, out int planId))
                Response.Redirect("StudentPlan2.aspx?studentId=" + studentId.Value + "&planId=" + planId, false);
        }

        private void LoadPlanListForStudent(int studentId)
        {
            var today = KuwaitTime.Today;
            var plans = db.StudentPlans.Where(p => p.StudentId == studentId && !p.IsArchived).OrderBy(p => p.PlanFromDate).ToList();
            PlanNameDropDown.DataSource = plans.Select(p => new { p.Id, Display = p.Name + " (" + p.PlanFromDate.ToString("yyyy-MM-dd") + " - " + p.PlanToDate.ToString("yyyy-MM-dd") + ")" }).ToList();
            PlanNameDropDown.DataValueField = "Id";
            PlanNameDropDown.DataTextField = "Display";
            PlanNameDropDown.DataBind();
            if (IsPostBack)
            {
                // Restore user's selection from postback so PlanNameDropDown_SelectedIndexChanged redirects with the new planId
                string postedPlanId = Request.Form[PlanNameDropDown.UniqueID];
                if (!string.IsNullOrEmpty(postedPlanId) && PlanNameDropDown.Items.FindByValue(postedPlanId) != null)
                    PlanNameDropDown.SelectedValue = postedPlanId;
            }
            else
            {
                int? planId = ViewPlanId;
                if (planId.HasValue && PlanNameDropDown.Items.FindByValue(planId.Value.ToString()) != null)
                    PlanNameDropDown.SelectedValue = planId.Value.ToString();
                else if (plans.Count > 0)
                {
                    var currentPlan = plans.FirstOrDefault(p => today >= p.PlanFromDate && today <= p.PlanToDate) ?? plans.First();
                    PlanNameDropDown.SelectedValue = currentPlan.Id.ToString();
                }
            }
        }

        /// <summary>
        /// Set مستوى الحفظ, تاريخ بداية الخطة, تاريخ نهاية الخطة from the plan's rows.
        /// </summary>
        private void LoadPlanHeaderFromDatabase(int planId)
        {
            var memRows = db.StudentPlanMemorizings.Where(x => x.PlanId == planId).Select(x => new { x.PlanDate, x.PlanEndDate, x.MemorizationLevel }).ToList();
            var revRows = db.StudentPlanRevises.Where(x => x.PlanId == planId).Select(x => new { x.PlanDate, x.PlanEndDate, x.MemorizationLevel }).ToList();
            var all = memRows.Concat(revRows).ToList();
            if (all.Count == 0) return;

            var minDate = all.Min(x => x.PlanDate);
            var maxDate = all.Max(x => x.PlanEndDate ?? x.PlanDate);
            var latest = all.OrderByDescending(x => x.PlanDate).First();
            string level = latest.MemorizationLevel;

        }

        protected void EditModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (EditModeCheckBox != null)
                EditMode = EditModeCheckBox.Checked;
            if (ViewStudentId.HasValue && ViewPlanId.HasValue)
                LoadPlansForStudent(ViewStudentId.Value, ViewPlanId.Value);
        }

        protected void LevelDropDownPlan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ViewStudentId.HasValue && ViewPlanId.HasValue)
                LoadPlansForStudent(ViewStudentId.Value, ViewPlanId.Value);
        }



        private void LoadStudents()
        {
            int circleId = user.CircleId();
            if (circleId <= 0)
            {
                StudentsRepeater.DataSource = Enumerable.Empty<object>();
                StudentsRepeater.DataBind();
                return;
            }

            var list = db.RegisterForms
                .Where(x => x.QuranCircleId == circleId)
                .OrderBy(x => x.FullName ?? x.StudentName)
                .Select(x => new { x.Id, Name = x.FullName ?? x.StudentName ?? "—", x.QuranCircleId }).ToList();
            StudentsRepeater.DataSource = list;
            StudentsRepeater.DataBind();
        }

        protected void StudentsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
        }

        private List<PlanRowData> ExpandSurahId(int inputId, int fromAyah, int toAyah, string planType)
        {
            var list = new List<PlanRowData>();
            if (inputId <= 114) { list.Add(new PlanRowData { SurahId = inputId, FromAyah = fromAyah, ToAyah = toAyah, PlanType = planType }); return list; }
            using (var db2 = new Model1())
            {
                IQueryable<Holyquran> q = db2.Holyqurans;
                if (inputId > 1000 && inputId <= 1100) { int hezb = inputId - 1000; q = q.Where(x => x.hezb_no == hezb); }
                else if (inputId > 2000 && inputId <= 2300) { int hezb = ((inputId - 2001) / 4) + 1; int quarter = ((inputId - 2001) % 4) + 1; q = q.Where(x => x.hezb_no == hezb && x.hezb_quarter == quarter); }
                else if (inputId > 3000 && inputId <= 3030) { int juz = inputId - 3000; q = q.Where(x => x.jozz == juz); }
                var items = q.GroupBy(x => x.sura_no).Select(g => new { sura_no = g.Key, min = g.Min(a => a.aya_no), max = g.Max(a => a.aya_no) }).ToList();
                foreach (var item in items) { list.Add(new PlanRowData { SurahId = item.sura_no, FromAyah = item.min, ToAyah = item.max, PlanType = planType }); }
            }
            return list;
        }

        private object GetExpandedSurahsList()
        {
            var items = db.QuranSurahs.OrderBy(x => x.SortOrder ?? x.Id).Select(x => new { Id = x.Id, NameAr = "سورة " + x.NameAr }).ToList();
            return items;
        }

        private void LoadSurahs()
        {
            var surahs = GetExpandedSurahsList();


            // Bind By-Level surah dropdowns with only actual surahs (1..114)
            var surahOnly = db.QuranSurahs
                .OrderBy(x => x.SortOrder ?? x.Id)
                .Select(x => new { Id = x.Id, NameAr = "سورة " + x.NameAr })
                .ToList();
            if (ByLevelSurahFromDropDown != null)
            {
                ByLevelSurahFromDropDown.DataSource = surahOnly;
                ByLevelSurahFromDropDown.DataValueField = "Id";
                ByLevelSurahFromDropDown.DataTextField = "NameAr";
                ByLevelSurahFromDropDown.DataBind();
                ByLevelSurahFromDropDown.Items.Insert(0, new ListItem("-- اختر سورة البداية --", ""));
            }
            if (ByLevelSurahToDropDown != null)
            {
                ByLevelSurahToDropDown.DataSource = surahOnly;
                ByLevelSurahToDropDown.DataValueField = "Id";
                ByLevelSurahToDropDown.DataTextField = "NameAr";
                ByLevelSurahToDropDown.DataBind();
                ByLevelSurahToDropDown.Items.Insert(0, new ListItem("-- اختر سورة النهاية --", ""));
            }
        }





        private void BindPlanRows()
        {
            var rows = new List<object>();
            for (int i = 0; i < PlanRowCount; i++) rows.Add(new { Index = i, MemorizeDate = (DateTime?)null, ReviseDate = (DateTime?)null });
            TabSingleCtrl.ThePlanRowsRepeater.DataSource = rows;
            TabSingleCtrl.ThePlanRowsRepeater.DataBind();

            string editKey = ViewState["EditPlanKey"] as string;
            var savedRows = ViewState["PlanRowsData"] as List<PlanRowData>;

            for (int i = 0; i < TabSingleCtrl.ThePlanRowsRepeater.Items.Count; i++)
            {
                RepeaterItem item = TabSingleCtrl.ThePlanRowsRepeater.Items[i];
                var surahDdl = (DropDownList)item.FindControl("SurahDropDown");
                if (surahDdl != null)
                {
                    var surahsList = GetExpandedSurahsList();
                    surahDdl.DataSource = surahsList;
                    surahDdl.DataValueField = "Id";
                    surahDdl.DataTextField = "NameAr";
                    surahDdl.DataBind();
                    surahDdl.Items.Insert(0, new ListItem("-- اختر السورة --", ""));
                }
                var typeDdl = (DropDownList)item.FindControl("TypeDropDown");
                if (typeDdl != null) typeDdl.SelectedValue = TabSingleCtrl.PlanType ?? "حفظ";

                if (!string.IsNullOrEmpty(editKey) && i == 0)
                    PrefillRowForEdit(item, editKey);
                else if (savedRows != null && i < savedRows.Count && savedRows[i] != null)
                    ApplyRowData(item, savedRows[i]);
            }

            if (savedRows != null)
                ViewState["PlanRowsData"] = null;
        }

        private void BindPlanRowsRevise()
        {
            var rows = new List<object>();
            for (int i = 0; i < PlanRowCountRevise; i++) rows.Add(new { Index = i, MemorizeDate = (DateTime?)null, ReviseDate = (DateTime?)null });
            TabReviseCtrl.ThePlanRowsRepeater.DataSource = rows;
            TabReviseCtrl.ThePlanRowsRepeater.DataBind();

            for (int i = 0; i < TabReviseCtrl.ThePlanRowsRepeater.Items.Count; i++)
            {
                RepeaterItem item = TabReviseCtrl.ThePlanRowsRepeater.Items[i];
                var surahDdl = (DropDownList)item.FindControl("SurahDropDown");
                if (surahDdl != null)
                {
                    var surahsList = GetExpandedSurahsList();
                    surahDdl.DataSource = surahsList;
                    surahDdl.DataValueField = "Id";
                    surahDdl.DataTextField = "NameAr";
                    surahDdl.DataBind();
                    surahDdl.Items.Insert(0, new ListItem("-- اختر السورة --", ""));
                }
                var typeDdl = (DropDownList)item.FindControl("TypeDropDown");
                if (typeDdl != null) typeDdl.SelectedValue = TabReviseCtrl.PlanType ?? "مراجعة";
            }
        }

        private void ApplyRowData(RepeaterItem item, PlanRowData data)
        {
            var surahDdl = (DropDownList)item.FindControl("SurahDropDown");
            var fromDdl = (DropDownList)item.FindControl("FromAyahDropDown");
            var toDdl = (DropDownList)item.FindControl("ToAyahDropDown");
            var typeDdl = (DropDownList)item.FindControl("TypeDropDown");
            if (surahDdl == null || fromDdl == null || toDdl == null || data.SurahId <= 0) return;

            surahDdl.SelectedValue = data.SurahId.ToString();
            var ayahs = db.QuranAyahs.Where(x => x.SurahId == data.SurahId).OrderBy(x => x.AyahNumber).Select(x => x.AyahNumber).ToList();
            fromDdl.Items.Clear();
            toDdl.Items.Clear();
            fromDdl.Items.Add(new ListItem("--", ""));
            toDdl.Items.Add(new ListItem("--", ""));
            foreach (int a in ayahs)
            {
                fromDdl.Items.Add(new ListItem(a.ToString(), a.ToString()));
                toDdl.Items.Add(new ListItem(a.ToString(), a.ToString()));
            }
            fromDdl.SelectedValue = data.FromAyah.ToString();
            toDdl.SelectedValue = data.ToAyah.ToString();
            if (typeDdl != null) typeDdl.SelectedValue = data.PlanType ?? "حفظ";
        }

        private void PrefillRowForEdit(RepeaterItem item, string editKey)
        {
            if (editKey.StartsWith("memorizing_") && int.TryParse(editKey.Substring("memorizing_".Length), out int memId))
            {
                var ent = db.StudentPlanMemorizings.Include(x => x.QuranSurah).FirstOrDefault(x => x.Id == memId);
                if (ent != null)
                {
                    var surahDdl = (DropDownList)item.FindControl("SurahDropDown");
                    var typeDdl = (DropDownList)item.FindControl("TypeDropDown");
                    if (surahDdl != null) surahDdl.SelectedValue = ent.SurahId.ToString();
                    if (typeDdl != null) typeDdl.SelectedValue = "حفظ";
                    var fromDdl = (DropDownList)item.FindControl("FromAyahDropDown");
                    var toDdl = (DropDownList)item.FindControl("ToAyahDropDown");
                    if (fromDdl != null && toDdl != null)
                    {
                        fromDdl.Items.Clear();
                        toDdl.Items.Clear();
                        var ayahs = db.QuranAyahs.Where(x => x.SurahId == ent.SurahId).OrderBy(x => x.AyahNumber).Select(x => x.AyahNumber).ToList();
                        foreach (int a in ayahs) { fromDdl.Items.Add(new ListItem(a.ToString(), a.ToString())); toDdl.Items.Add(new ListItem(a.ToString(), a.ToString())); }
                        fromDdl.SelectedValue = ent.FromAyahNumber.ToString();
                        toDdl.SelectedValue = ent.ToAyahNumber.ToString();
                    }
                }
            }
            else if (editKey.StartsWith("revise_") && int.TryParse(editKey.Substring("revise_".Length), out int revId))
            {
                var ent = db.StudentPlanRevises.Include(x => x.QuranSurah).FirstOrDefault(x => x.Id == revId);
                if (ent != null)
                {
                    var surahDdl = (DropDownList)item.FindControl("SurahDropDown");
                    var typeDdl = (DropDownList)item.FindControl("TypeDropDown");
                    if (surahDdl != null) surahDdl.SelectedValue = ent.SurahId.ToString();
                    if (typeDdl != null) typeDdl.SelectedValue = "مراجعة";
                    var fromDdl = (DropDownList)item.FindControl("FromAyahDropDown");
                    var toDdl = (DropDownList)item.FindControl("ToAyahDropDown");
                    if (fromDdl != null && toDdl != null)
                    {
                        fromDdl.Items.Clear();
                        toDdl.Items.Clear();
                        var ayahs = db.QuranAyahs.Where(x => x.SurahId == ent.SurahId).OrderBy(x => x.AyahNumber).Select(x => x.AyahNumber).ToList();
                        foreach (int a in ayahs) { fromDdl.Items.Add(new ListItem(a.ToString(), a.ToString())); toDdl.Items.Add(new ListItem(a.ToString(), a.ToString())); }
                        fromDdl.SelectedValue = ent.FromAyahNumber.ToString();
                        toDdl.SelectedValue = ent.ToAyahNumber.ToString();
                    }
                }
            }
        }

        protected void AddRowButton_Click(object sender, EventArgs e)
        {
            var savedRows = new List<PlanRowData>();
            foreach (RepeaterItem item in TabSingleCtrl.ThePlanRowsRepeater.Items)
            {
                if (GetRowValuesFromForm(item, out int surahId, out int fromAyah, out int toAyah, out string planType))
                    savedRows.Add(new PlanRowData { SurahId = surahId, FromAyah = fromAyah, ToAyah = toAyah, PlanType = planType ?? "حفظ" });
                else
                    savedRows.Add(null);
            }
            ViewState["PlanRowsData"] = savedRows;

            PlanRowCount++;
            LoadSurahs();
            BindPlanRows();
            ScriptManager.RegisterStartupScript(this, GetType(), "ReinitPlanDropdowns", "if (typeof window.reinitPlanSearchableDropdowns === 'function') window.reinitPlanSearchableDropdowns(); if (typeof window.restorePlanRowsFromStorage === 'function') window.restorePlanRowsFromStorage();", true);
        }

        protected void AddReviseRowButton_Click(object sender, EventArgs e)
        {
            PlanRowCountRevise++;
            LoadSurahs();
            BindPlanRowsRevise();
            ScriptManager.RegisterStartupScript(this, GetType(), "ReinitReviseDropdowns",
                "if (typeof window.reinitPlanSearchableDropdowns === 'function') window.reinitPlanSearchableDropdowns();", true);
        }

        protected void SaveReviseButton_Click(object sender, EventArgs e)
        {
            int? studentId = ViewStudentId;
            int? planId = ViewPlanId;

            if (!studentId.HasValue || !planId.HasValue)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ReviseErr",
                    "alert('\u064a\u0631\u062c\u0649 \u0627\u062e\u062a\u064a\u0627\u0631 \u0637\u0627\u0644\u0628 \u0648\u062e\u0637\u0629 \u0623\u0648\u0644\u0627\u064b.');", true);
                return;
            }

            // Parse revise date
            DateTime reviseDate = KuwaitTime.Today;
            if (ReviseDate != null && !string.IsNullOrEmpty(ReviseDate.Text) &&
                DateTime.TryParse(ReviseDate.Text, out DateTime parsedRevise))
                reviseDate = parsedRevise.Date;


            var now = KuwaitTime.Now;
            int savedCount = 0;

            try
            {
                var planEndDate = db.StudentPlans.Where(p => p.Id == planId.Value)
                    .Select(p => p.PlanToDate).FirstOrDefault();

                string level = db.StudentPlanRevises.Where(x => x.PlanId == planId.Value).Select(x => x.MemorizationLevel).FirstOrDefault()
                    ?? db.StudentPlanMemorizings.Where(x => x.PlanId == planId.Value).Select(x => x.MemorizationLevel).FirstOrDefault()
                    ?? "—";

                foreach (RepeaterItem item in TabReviseCtrl.ThePlanRowsRepeater.Items)
                {
                    var surahDdl = (DropDownList)item.FindControl("SurahDropDown");
                    var fromDdl = (DropDownList)item.FindControl("FromAyahDropDown");
                    var toDdl = (DropDownList)item.FindControl("ToAyahDropDown");

                    if (surahDdl == null) continue;
                    string surahVal = Request.Form[surahDdl.UniqueID];
                    string fromVal = fromDdl != null ? Request.Form[fromDdl.UniqueID] : null;
                    string toVal = toDdl != null ? Request.Form[toDdl.UniqueID] : null;

                    if (string.IsNullOrEmpty(surahVal)) continue;
                    if (!int.TryParse(surahVal, out int surahId) || surahId <= 0) continue;
                    if (string.IsNullOrEmpty(fromVal) || string.IsNullOrEmpty(toVal)) continue;
                    if (!int.TryParse(fromVal, out int fromAyah)) continue;
                    if (!int.TryParse(toVal, out int toAyah)) continue;

                    db.StudentPlanRevises.Add(new masabihq8Shared.StudentPlanRevise
                    {
                        StudentId = studentId.Value,
                        PlanId = planId.Value,
                        MemorizationLevel = level,
                        SurahId = surahId,
                        FromAyahNumber = fromAyah,
                        ToAyahNumber = toAyah,
                        PlanDate = reviseDate,
                        PlanEndDate = planEndDate != default ? planEndDate : reviseDate,
                        CreatedAt = now,
                        Status = StatusPending
                    });
                    savedCount++;
                }

                if (savedCount == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ReviseEmpty",
                        "alert('\u064a\u0631\u062c\u0649 \u0625\u0636\u0627\u0641\u0629 \u0633\u0637\u0631 \u0645\u0631\u0627\u062c\u0639\u0629 \u0648\u0627\u062d\u062f \u0639\u0644\u0649 \u0627\u0644\u0623\u0642\u0644 \u0648\u062a\u062d\u062f\u064a\u062f \u0627\u0644\u0633\u0648\u0631\u0629 \u0648\u0627\u0644\u0622\u064a\u0627\u062a.');", true);
                    return;
                }

                db.SaveChanges();

                // Reset revise rows count
                PlanRowCountRevise = 1;
                BindPlanRowsRevise();

                ScriptManager.RegisterStartupScript(this, GetType(), "ReviseOk",
                    "alert('\u062a\u0645 \u062d\u0641\u0638 \u0627\u0644\u0645\u0631\u0627\u062c\u0639\u0629 \u0628\u0646\u062c\u0627\u062d.'); window.location = 'StudentPlan2.aspx?studentId=" + studentId.Value + "&planId=" + planId.Value + "&tab=tab-revise';", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ReviseErr2",
                    "alert('\u062e\u0637\u0623: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        protected void SaveByLevelSurahsButton_Click(object sender, EventArgs e)
        {
            // 1. التحقق من المدخلات الأساسية
            if (string.IsNullOrEmpty(ByLevelSurahFromDropDown.SelectedValue) || string.IsNullOrEmpty(ByLevelSurahToDropDown.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ByLevelRangeErr", "alert('يرجى اختيار سورة البداية والنهاية.');", true);
                return;
            }

            // 2. تحديد السور المستهدفة
            int fromSurahId = int.Parse(ByLevelSurahFromDropDown.SelectedValue);
            int toSurahId = int.Parse(ByLevelSurahToDropDown.SelectedValue);
            string planType = ByLevelTypeDropDown.SelectedValue;

            var allSurahs = db.QuranSurahs.OrderBy(x => x.SortOrder ?? x.Id).ToList();
            var fromSurah = allSurahs.FirstOrDefault(s => s.Id == fromSurahId);
            var toSurah = allSurahs.FirstOrDefault(s => s.Id == toSurahId);

            if (fromSurah == null || toSurah == null) return;

            int fromOrder = fromSurah.SortOrder ?? fromSurah.Id;
            int toOrder = toSurah.SortOrder ?? toSurah.Id;
            int minOrder = Math.Min(fromOrder, toOrder);
            int maxOrder = Math.Max(fromOrder, toOrder);
            var targetSurahs = allSurahs.Where(s => (s.SortOrder ?? s.Id) >= minOrder && (s.SortOrder ?? s.Id) <= maxOrder).ToList();
            var targetSurahIds = targetSurahs.Select(s => s.Id).ToList();

            // 3. جلب بيانات المصحف كاملة للنطاق المختار لتحسين الأداء
            var allQuranData = db.Holyqurans
                .Where(h => targetSurahIds.Contains(h.sura_no))
                .OrderBy(h => h.sura_no).ThenBy(h => h.aya_no)
                .ToList();

            var rowsWithDates = new List<Tuple<int, int, int, DateTime>>();

            var userAcc = new TheUser();
            int circleId = userAcc.CircleId();
            var circleDayNumbers = db.CircleDays
                .Where(d => d.CircleId == circleId)
                .Select(d => d.DayNumber)
                .ToList();

            // تحديد تاريخ البداية
            DateTime currentDate = KuwaitTime.Today;

            // Ensure first day is a circle day if possible
            if (circleDayNumbers.Any() && !circleDayNumbers.Contains((int)currentDate.DayOfWeek))
            {
                currentDate = GetNextCircleDayForLogic(currentDate, circleDayNumbers);
                // But we want to START on this day if it's the first one, or the one after?
                // Usually we want the first date to be the start provided, unless it's not a circle day.
            }

            bool usedLevelDivision = false;


            // 4. حالة السقوط (Fallback) إذا لم يحدد مستوى
            if (!usedLevelDivision)
            {
                foreach (var s in targetSurahs)
                {
                    var sData = allQuranData.Where(h => h.sura_no == s.Id).ToList();
                    if (sData.Any())
                    {
                        rowsWithDates.Add(Tuple.Create(s.Id, sData.Min(h => h.aya_no), sData.Max(h => h.aya_no), currentDate));
                        currentDate = GetNextCircleDayForLogic(currentDate, circleDayNumbers);
                    }
                }
            }

            // 5. تحديد الطلاب وحفظ الخطة
            var selectedStudents = GetSelectedStudentIds(); // وظيفة مساعدة لجلب ID الطلاب
            if (!selectedStudents.Any())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "NoStudents", "alert('يرجى اختيار طالب واحد على الأقل.');", true);
                return;
            }

            try
            {
                foreach (int sid in selectedStudents)
                {
                    // إنشاء رأس الخطة
                    var plan = new masabihq8Shared.StudentPlan
                    {
                        StudentId = sid,
                        PlanFromDate = rowsWithDates.First().Item4,
                        PlanToDate = rowsWithDates.Last().Item4, // تاريخ النهاية الفعلي بناءً على التقسيم
                        CreatedAt = KuwaitTime.Now
                    };
                    db.StudentPlans.Add(plan);
                    db.SaveChanges();

                    foreach (var row in rowsWithDates)
                    {
                        if (planType.Contains("حفظ"))
                        {
                            db.StudentPlanMemorizings.Add(new StudentPlanMemorizing
                            {
                                StudentId = sid,
                                PlanId = plan.Id,
                                SurahId = row.Item1,
                                FromAyahNumber = row.Item2,
                                ToAyahNumber = row.Item3,
                                PlanDate = row.Item4,
                                PlanEndDate = row.Item4,
                                CreatedAt = KuwaitTime.Now
                            });
                        }
                        if (planType.Contains("مراجعة"))
                        {
                            db.StudentPlanRevises.Add(new StudentPlanRevise
                            {
                                StudentId = sid,
                                PlanId = plan.Id,
                                SurahId = row.Item1,
                                FromAyahNumber = row.Item2,
                                ToAyahNumber = row.Item3,
                                PlanDate = row.Item4,
                                PlanEndDate = row.Item4,
                                CreatedAt = KuwaitTime.Now
                            });
                        }
                    }
                }
                db.SaveChanges();
                ScriptManager.RegisterStartupScript(this, GetType(), "Success", "alert('تم توزيع السور بنجاح بناءً على معايير المستوى.'); window.location.href='StudentPlan2.aspx';", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", $"alert('حدث خطأ: {ex.Message}');", true);
            }
        }

        // وظيفة مساعدة لجلب معرفات الطلاب المختارين
        private List<int> GetSelectedStudentIds()
        {
            var ids = new List<int>();
            if (ViewStudentId.HasValue) ids.Add(ViewStudentId.Value);
            else if (!string.IsNullOrEmpty(SelectedStudentIds?.Value))
            {
                ids.AddRange(SelectedStudentIds.Value.Split(',').Select(int.Parse));
            }
            return ids;
        }


        protected void SavePlanButton_Click(object sender, EventArgs e)
        {
            string editKey = ViewState["EditPlanKey"] as string;
            if (!string.IsNullOrEmpty(editKey))
            {
                SavePlanUpdateSingle(editKey);
                return;
            }

            var selectedStudents = new List<int>();
            if (ViewStudentId.HasValue)
            {
                selectedStudents.Add(ViewStudentId.Value);
            }
            else if (PanelStudentsSelect.Visible && SelectedStudentIds != null && !string.IsNullOrEmpty(SelectedStudentIds.Value))
            {
                foreach (var idStr in SelectedStudentIds.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    if (int.TryParse(idStr.Trim(), out int sid))
                        selectedStudents.Add(sid);
            }
            else if (PanelStudentsSelect.Visible && StudentsRepeater != null)
            {
                foreach (RepeaterItem item in StudentsRepeater.Items)
                {
                    var cb = (CheckBox)item.FindControl("StudentCheckBox");
                    var hid = (HiddenField)item.FindControl("StudentIdHidden");
                    if (cb != null && cb.Checked && hid != null && !string.IsNullOrEmpty(hid.Value) && int.TryParse(hid.Value, out int sid))
                        selectedStudents.Add(sid);
                }
            }

            if (selectedStudents.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "NoStudents", "alert('يرجى اختيار طالب واحد على الأقل.');", true);
                return;
            }

            DateTime planStart = KuwaitTime.Today;
            DateTime planEnd = KuwaitTime.Today;

            var now = KuwaitTime.Now;
            if (EditMode && ViewStudentId.HasValue && TabSingleCtrl.ThePlansRepeaterEdit.Visible && TabSingleCtrl.ThePlansRepeaterEdit.Items.Count > 0)
            {
                int editStudentId = ViewStudentId.Value;
                foreach (RepeaterItem item in TabSingleCtrl.ThePlansRepeaterEdit.Items)
                {
                    var keyHid = (HiddenField)item.FindControl("PlanKeyHidden");
                    string rowKey = keyHid?.Value;
                    if (string.IsNullOrEmpty(rowKey)) continue;
                    if (!GetEditRowValues(item, out int surahId, out int fromAyah, out int toAyah, out string planType))
                        continue;
                    if (string.IsNullOrEmpty(planType)) planType = "حفظ";
                    try
                    {
                        bool wasMemorizing = rowKey.StartsWith("memorizing_");
                        bool wasRevise = rowKey.StartsWith("revise_");
                        bool wantMemorizing = planType == "حفظ" ;
                        bool wantRevise = planType == "مراجعة" ;

                        if (wasMemorizing && int.TryParse(rowKey.Substring("memorizing_".Length), out int memId))
                        {
                            var memEnt = db.StudentPlanMemorizings.Find(memId);
                            if (memEnt != null)
                            {
                                if (wantMemorizing && !wantRevise)
                                {
                                    memEnt.SurahId = surahId;
                                    memEnt.FromAyahNumber = fromAyah;
                                    memEnt.ToAyahNumber = toAyah;
                                    memEnt.PlanDate = planStart;
                                    memEnt.PlanEndDate = planEnd;
                                }
                                else if (wantRevise && !wantMemorizing)
                                {
                                    int editPlanId = memEnt.PlanId;
                                    db.StudentPlanMemorizings.Remove(memEnt);
                                    db.StudentPlanRevises.Add(new StudentPlanRevise
                                    {
                                        StudentId = editStudentId,
                                        PlanId = editPlanId,
                                        SurahId = surahId,
                                        FromAyahNumber = fromAyah,
                                        ToAyahNumber = toAyah,
                                        PlanDate = planStart,
                                        PlanEndDate = planEnd,
                                        CreatedAt = now
                                    });
                                }
                                else
                                {
                                    int editPlanId = memEnt.PlanId;
                                    memEnt.SurahId = surahId;
                                    memEnt.FromAyahNumber = fromAyah;
                                    memEnt.ToAyahNumber = toAyah;
                                    memEnt.PlanDate = planStart;
                                    memEnt.PlanEndDate = planEnd;
                                    db.StudentPlanRevises.Add(new StudentPlanRevise
                                    {
                                        StudentId = editStudentId,
                                        PlanId = editPlanId,
                                        SurahId = surahId,
                                        FromAyahNumber = fromAyah,
                                        ToAyahNumber = toAyah,
                                        PlanDate = planStart,
                                        PlanEndDate = planEnd,
                                        CreatedAt = now
                                    });
                                }
                            }
                        }
                        else if (wasRevise && int.TryParse(rowKey.Substring("revise_".Length), out int revId))
                        {
                            var revEnt = db.StudentPlanRevises.Find(revId);
                            if (revEnt != null)
                            {
                                if (wantRevise && !wantMemorizing)
                                {
                                    revEnt.SurahId = surahId;
                                    revEnt.FromAyahNumber = fromAyah;
                                    revEnt.ToAyahNumber = toAyah;
                                    revEnt.PlanDate = planStart;
                                    revEnt.PlanEndDate = planEnd;
                                }
                                else if (wantMemorizing && !wantRevise)
                                {
                                    int editPlanId = revEnt.PlanId;
                                    db.StudentPlanRevises.Remove(revEnt);
                                    db.StudentPlanMemorizings.Add(new StudentPlanMemorizing
                                    {
                                        StudentId = editStudentId,
                                        PlanId = editPlanId,
                                        SurahId = surahId,
                                        FromAyahNumber = fromAyah,
                                        ToAyahNumber = toAyah,
                                        PlanDate = planStart,
                                        PlanEndDate = planEnd,
                                        CreatedAt = now
                                    });
                                }
                                else
                                {
                                    int editPlanId = revEnt.PlanId;
                                    revEnt.SurahId = surahId;
                                    revEnt.FromAyahNumber = fromAyah;
                                    revEnt.ToAyahNumber = toAyah;
                                    revEnt.PlanDate = planStart;
                                    revEnt.PlanEndDate = planEnd;
                                    db.StudentPlanMemorizings.Add(new StudentPlanMemorizing
                                    {
                                        StudentId = editStudentId,
                                        PlanId = editPlanId,
                                        SurahId = surahId,
                                        FromAyahNumber = fromAyah,
                                        ToAyahNumber = toAyah,
                                        PlanDate = planStart,
                                        PlanEndDate = planEnd,
                                        CreatedAt = now
                                    });
                                }
                            }
                        }
                    }
                    catch { /* skip invalid row */ }
                }
            }

            int? singlePlanId = ViewPlanId;
            var studentToPlanId = new Dictionary<int, int>();
            if (singlePlanId.HasValue && selectedStudents.Count == 1 && selectedStudents[0] == ViewStudentId)
            {
                studentToPlanId[selectedStudents[0]] = singlePlanId.Value;
            }
            else
            {
                foreach (int sid in selectedStudents)
                {
                    var plan = new masabihq8Shared.StudentPlan
                    {
                        StudentId = sid,
                        Name = "خطة جديدة " + planStart.ToString("yyyy-MM-dd"),
                        PlanFromDate = planStart,
                        PlanToDate = planEnd,
                        CreatedAt = now
                    };
                    db.StudentPlans.Add(plan);
                    db.SaveChanges();
                    studentToPlanId[sid] = plan.Id;
                }
            }

            var expandedRows = new List<PlanRowData>();
            foreach (RepeaterItem item in TabSingleCtrl.ThePlanRowsRepeater.Items)
            {
                if (!GetRowValuesFromForm(item, out int surahId, out int fromAyah, out int toAyah, out string planType))
                    continue;
                expandedRows.AddRange(ExpandSurahId(surahId, fromAyah, toAyah, planType));
            }

            foreach (var row in expandedRows)
            {
                int surahId = row.SurahId;
                int fromAyah = row.FromAyah;
                int toAyah = row.ToAyah;
                string planType = row.PlanType;

                foreach (int studentId in selectedStudents)
                {
                    int planId = studentToPlanId[studentId];
                    if (planType == "مراجعة")
                    {
                        db.StudentPlanRevises.Add(new StudentPlanRevise
                        {
                            StudentId = studentId,
                            PlanId = planId,
                            SurahId = surahId,
                            FromAyahNumber = fromAyah,
                            ToAyahNumber = toAyah,
                            PlanDate = planStart,
                            PlanEndDate = planEnd,
                            CreatedAt = now
                        });
                    }
                    
                    else
                    {
                        db.StudentPlanMemorizings.Add(new StudentPlanMemorizing
                        {
                            StudentId = studentId,
                            PlanId = planId,
                            SurahId = surahId,
                            FromAyahNumber = fromAyah,
                            ToAyahNumber = toAyah,
                            PlanDate = planStart,
                            PlanEndDate = planEnd,
                            CreatedAt = now
                        });
                    }
                }
            }

            try
            {
                db.SaveChanges();
                if (ViewStudentId.HasValue && singlePlanId.HasValue)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "SaveOk", "try { localStorage.removeItem('StudentPlanRows'); } catch(e) {} alert('تم حفظ الخطة بنجاح.'); window.location = 'StudentPlan2.aspx?studentId=" + ViewStudentId.Value + "&planId=" + singlePlanId.Value + "';", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "SaveOk", "try { localStorage.removeItem('StudentPlanRows'); } catch(e) {} alert('تم حفظ الخطة بنجاح.');", true);
                    if (ViewStudentId.HasValue && ViewPlanId.HasValue)
                        LoadPlansForStudent(ViewStudentId.Value, ViewPlanId.Value);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SaveErr", "alert('خطأ: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        private bool GetRowValues(RepeaterItem item, out int surahId, out int fromAyah, out int toAyah, out string planType)
        {
            surahId = 0; fromAyah = 0; toAyah = 0; planType = "حفظ";
            var surahDdl = (DropDownList)item.FindControl("SurahDropDown");
            var fromDdl = (DropDownList)item.FindControl("FromAyahDropDown");
            var toDdl = (DropDownList)item.FindControl("ToAyahDropDown");
            if (surahDdl == null || string.IsNullOrEmpty(surahDdl.SelectedValue)) return false;
            if (fromDdl == null || toDdl == null || string.IsNullOrEmpty(fromDdl.SelectedValue) || string.IsNullOrEmpty(toDdl.SelectedValue)) return false;
            if (!int.TryParse(surahDdl.SelectedValue, out surahId)) return false;
            if (surahId > 1000) { fromAyah = 1; toAyah = 1; }
            else
            {
                if (!int.TryParse(fromDdl.SelectedValue, out fromAyah)) return false;
                if (!int.TryParse(toDdl.SelectedValue, out toAyah)) return false;
            }
            var typeDdl = (DropDownList)item.FindControl("TypeDropDown");
            planType = typeDdl?.SelectedValue ?? "حفظ";
            return true;
        }

        /// <summary>
        /// Read row values from Request.Form so we get the posted values even when
        /// from/to ayah dropdowns don't have options on the server (they are filled client-side).
        /// </summary>
        private bool GetRowValuesFromForm(RepeaterItem item, out int surahId, out int fromAyah, out int toAyah, out string planType)
        {
            surahId = 0; fromAyah = 0; toAyah = 0; planType = "حفظ";
            var surahDdl = (DropDownList)item.FindControl("SurahDropDown");
            var fromDdl = (DropDownList)item.FindControl("FromAyahDropDown");
            var toDdl = (DropDownList)item.FindControl("ToAyahDropDown");
            var typeDdl = (DropDownList)item.FindControl("TypeDropDown");
            if (surahDdl == null || fromDdl == null || toDdl == null) return false;
            string surahVal = Request.Form[surahDdl.UniqueID];
            string fromVal = Request.Form[fromDdl.UniqueID];
            string toVal = Request.Form[toDdl.UniqueID];
            if (string.IsNullOrEmpty(surahVal)) return false; if (int.TryParse(surahVal, out int tempSid) && tempSid <= 1000 && (string.IsNullOrEmpty(fromVal) || string.IsNullOrEmpty(toVal))) return false;
            if (!int.TryParse(surahVal, out surahId)) return false;
            if (surahId > 1000) { fromAyah = 1; toAyah = 1; }
            else
            {
                if (!int.TryParse(fromVal, out fromAyah)) return false;
                if (!int.TryParse(toVal, out toAyah)) return false;
            }
            planType = typeDdl != null ? (Request.Form[typeDdl.UniqueID] ?? typeDdl.SelectedValue) : "حفظ";
            if (string.IsNullOrEmpty(planType)) planType = "حفظ";
            return true;
        }

        /// <summary>
        /// Read edit-row values: try Request.Form first, then fall back to control SelectedValue
        /// (edit rows are server-bound so controls may have values even if form did not post them).
        /// </summary>
        private bool GetEditRowValues(RepeaterItem item, out int surahId, out int fromAyah, out int toAyah, out string planType)
        {
            surahId = 0; fromAyah = 0; toAyah = 0; planType = "حفظ";
            var surahDdl = (DropDownList)item.FindControl("SurahDropDown");
            var fromDdl = (DropDownList)item.FindControl("FromAyahDropDown");
            var toDdl = (DropDownList)item.FindControl("ToAyahDropDown");
            var typeDdl = (DropDownList)item.FindControl("TypeDropDown");
            if (surahDdl == null || fromDdl == null || toDdl == null) return false;
            string surahVal = Request.Form[surahDdl.UniqueID];
            string fromVal = Request.Form[fromDdl.UniqueID];
            string toVal = Request.Form[toDdl.UniqueID];
            if (string.IsNullOrEmpty(surahVal)) surahVal = surahDdl.SelectedValue;
            if (string.IsNullOrEmpty(fromVal)) fromVal = fromDdl.SelectedValue;
            if (string.IsNullOrEmpty(toVal)) toVal = toDdl.SelectedValue;
            if (string.IsNullOrEmpty(surahVal)) return false; if (int.TryParse(surahVal, out int tempSid) && tempSid <= 1000 && (string.IsNullOrEmpty(fromVal) || string.IsNullOrEmpty(toVal))) return false;
            if (!int.TryParse(surahVal, out surahId)) return false;
            if (surahId > 1000) { fromAyah = 1; toAyah = 1; }
            else
            {
                if (!int.TryParse(fromVal, out fromAyah)) return false;
                if (!int.TryParse(toVal, out toAyah)) return false;
            }
            planType = typeDdl != null ? (Request.Form[typeDdl?.UniqueID] ?? typeDdl.SelectedValue) : "حفظ";
            if (string.IsNullOrEmpty(planType)) planType = "حفظ";
            return true;
        }

        private void SavePlanUpdateSingle(string editKey)
        {
            if (TabSingleCtrl.ThePlanRowsRepeater.Items.Count == 0) return;
            RepeaterItem item = TabSingleCtrl.ThePlanRowsRepeater.Items[0];
            if (!GetRowValues(item, out int surahId, out int fromAyah, out int toAyah, out string planType))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "EditInvalid", "alert('يرجى تعبئة السورة و من آية وإلى آية.');", true);
                return;
            }
            DateTime planStart = KuwaitTime.Today;
            DateTime planEnd = KuwaitTime.Today;

            try
            {
                if (editKey.StartsWith("memorizing_") && int.TryParse(editKey.Substring("memorizing_".Length), out int memId))
                {
                    var ent = db.StudentPlanMemorizings.Find(memId);
                    if (ent != null)
                    {
                        ent.SurahId = surahId;
                        ent.FromAyahNumber = fromAyah;
                        ent.ToAyahNumber = toAyah;
                        ent.PlanDate = planStart;
                        ent.PlanEndDate = planEnd;
                        db.SaveChanges();
                        ViewState["EditPlanKey"] = null;
                        ScriptManager.RegisterStartupScript(this, GetType(), "UpdateOk", "alert('تم تحديث الخطة بنجاح.'); window.location = 'StudentPlan2.aspx?studentId=" + ent.StudentId + "&planId=" + ent.PlanId + "';", true);
                        return;
                    }
                }
                if (editKey.StartsWith("revise_") && int.TryParse(editKey.Substring("revise_".Length), out int revId))
                {
                    var ent = db.StudentPlanRevises.Find(revId);
                    if (ent != null)
                    {
                        ent.SurahId = surahId;
                        ent.FromAyahNumber = fromAyah;
                        ent.ToAyahNumber = toAyah;
                        ent.PlanDate = planStart;
                        ent.PlanEndDate = planEnd;
                        db.SaveChanges();
                        ViewState["EditPlanKey"] = null;
                        ScriptManager.RegisterStartupScript(this, GetType(), "UpdateOk", "alert('تم تحديث الخطة بنجاح.'); window.location = 'StudentPlan2.aspx?studentId=" + ent.StudentId + "&planId=" + ent.PlanId + "';", true);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "UpdateErr", "alert('خطأ: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        private void LoadPlansForStudent(int studentId, int planId)
        {
            var mem = db.StudentPlanMemorizings
                .Where(x => x.StudentId == studentId && x.PlanId == planId)
                .Include(x => x.QuranSurah)
                .OrderBy(x => x.PlanDate)
                .ToList()
                .Select(x => new PlanRowVM
                {
                    Key = "memorizing_" + x.Id,
                    PlanType = "حفظ",
                    MemorizationLevel = x.MemorizationLevel,
                    SurahId = x.SurahId,
                    SurahName = x.QuranSurah?.NameAr ?? "—",
                    FromAyahNumber = x.FromAyahNumber,
                    ToAyahNumber = x.ToAyahNumber,
                    PlanDate = x.PlanDate,
                    PlanDateFormatted = x.PlanDate.ToString("yyyy-MM-dd"),
                    LatestStatus = x.Status ?? StatusPending,
                    MemorizeDate = x.MemorizeDate,
                    ReviseDate = x.ReviseDate
                })
                .ToList();
            var rev = db.StudentPlanRevises
                .Where(x => x.StudentId == studentId && x.PlanId == planId)
                .Include(x => x.QuranSurah)
                .OrderBy(x => x.PlanDate)
                .ToList()
                .Select(x => new PlanRowVM
                {
                    Key = "revise_" + x.Id,
                    PlanType = "مراجعة",
                    MemorizationLevel = x.MemorizationLevel,
                    SurahId = x.SurahId,
                    SurahName = x.QuranSurah?.NameAr ?? "—",
                    FromAyahNumber = x.FromAyahNumber,
                    ToAyahNumber = x.ToAyahNumber,
                    PlanDate = x.PlanDate,
                    PlanDateFormatted = x.PlanDate.ToString("yyyy-MM-dd"),
                    LatestStatus = x.Status ?? StatusPending,
                    MemorizeDate = x.MemorizeDate,
                    ReviseDate = x.ReviseDate
                })
                .ToList();

            var combined = mem.Concat(rev).OrderBy(x => x.PlanDate).ToList();

            List<PlanRowVM> displayRows = combined;
            if (!EditMode)
            {
                int memPendingIndex = mem.FindIndex(x => x.LatestStatus == StatusPending);
                if (memPendingIndex >= 0)
                {
                    // Show all previous mem rows + the first pending mem row
                    displayRows = mem.Take(memPendingIndex + 1).ToList();
                }
                else
                {
                    int revPendingIndex = rev.FindIndex(x => x.LatestStatus == StatusPending);
                    if (revPendingIndex >= 0)
                    {
                        // All mem rows are considered "previous"; show them + first pending rev
                        displayRows = mem.Concat(rev.Take(revPendingIndex + 1)).ToList();
                    }
                    else
                    {
                        // No pending at all: show everything
                        displayRows = combined;
                    }
                }
            }

            // Single tab: show only ONE record - the first mem row with status قيد الانتظار. When تم الحفظ, show next pending.
            var firstPendingMem = mem.FirstOrDefault(x => x.LatestStatus == StatusPending);
            var displayRowsFiltered = firstPendingMem != null ? new List<PlanRowVM> { firstPendingMem } : new List<PlanRowVM>();
            TabSingleCtrl.ThePlansRepeater.DataSource = displayRowsFiltered;
            TabSingleCtrl.ThePlansRepeater.DataBind();
            TabSingleCtrl.ThePlansRepeaterEdit.DataSource = combined.Where(x => x.PlanType == "حفظ" && x.LatestStatus != StatusPendingTathbit && x.LatestStatus != StatusPass).ToList();
            TabSingleCtrl.ThePlansRepeaterEdit.DataBind();
            TabSingleCtrl.ThePlansRepeater.Visible = !EditMode;
            TabSingleCtrl.ThePlansRepeaterEdit.Visible = EditMode;
            TabSingleCtrl.ThePanelNoPlans.Visible = displayRowsFiltered.Count == 0;

            // Revise tab: show first record where Status is Pending or Fail. Keep لم يتم المراجعة visible for retry.
            var nextPendingRevise = rev.FirstOrDefault(x => x.LatestStatus == StatusPending || x.LatestStatus == StatusFail);
            var reviseOnlyRows = nextPendingRevise != null ? new List<PlanRowVM> { nextPendingRevise } : new List<PlanRowVM>();
            if (TabReviseCtrl.ThePlansRepeater != null)
            {
                TabReviseCtrl.ThePlansRepeater.DataSource = reviseOnlyRows;
                TabReviseCtrl.ThePlansRepeater.DataBind();
            }
            if (TabReviseCtrl.ThePanelNoPlans != null)
                TabReviseCtrl.ThePanelNoPlans.Visible = reviseOnlyRows.Count == 0;
            
            if (TabReviseCtrl != null) TabReviseCtrl.ShowActions = true;
            if (TabSingleCtrl != null) TabSingleCtrl.ShowActions = true;
            if (TabTathbitCtrl != null) TabTathbitCtrl.ShowActions = false;

            // Bind TabTathbitCtrl.ThePlansRepeater: items with status = تم OR قيد الانتظار في التثبيت
            var tathbitRows = combined.Where(x => x.LatestStatus == StatusPass || x.LatestStatus == StatusPendingTathbit).ToList();
            if (TabTathbitCtrl.ThePlansRepeater != null)
            {
                TabTathbitCtrl.ThePlansRepeater.DataSource = tathbitRows;
                TabTathbitCtrl.ThePlansRepeater.DataBind();
            }
            if (TabTathbitCtrl.ThePanelNoPlans != null)
                TabTathbitCtrl.ThePanelNoPlans.Visible = tathbitRows.Count == 0;

            LoadAssessmentLog(planId);
            SetDaysRemainingAndProgress(planId);

            // Build calendar tab
            if (ViewStudentId.HasValue)
                BuildCalendarData(ViewStudentId.Value, planId, combined);
        }

        private void BuildCalendarData(int studentId, int planId, List<PlanRowVM> allRows)
        {
            if (CalendarRepeater == null) return;
            try
            {
                var plan = db.StudentPlans.FirstOrDefault(p => p.Id == planId);
                if (plan == null) { CalendarRepeater.DataSource = null; CalendarRepeater.DataBind(); return; }

                // Get circle days for the student's circle
                var student = db.RegisterForms.FirstOrDefault(x => x.Id == studentId);
                List<int> circleDayNumbers = new List<int>();
                if (student != null && student.QuranCircleId.HasValue)
                {
                    circleDayNumbers = db.CircleDays
                        .Where(d => d.CircleId == student.QuranCircleId.Value)
                        .Select(d => d.DayNumber)
                        .ToList();
                }

                // Group plan rows by date
                var byDate = allRows
                    .GroupBy(r => r.PlanDate.Date)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Get base URL for Quran link
                string quranBase = VirtualPathUtility.ToAbsolute("~/Quran.aspx");

                var calDays = new List<CalendarDayVM>();
                DateTime start = plan.PlanFromDate.Date;
                DateTime end = plan.PlanToDate.Date;

                for (var d = start; d <= end; d = d.AddDays(1))
                {
                    bool isCircle = circleDayNumbers.Contains((int)d.DayOfWeek);
                    var vm = new CalendarDayVM { Date = d, IsCircleDay = isCircle };

                    if (isCircle && byDate.ContainsKey(d))
                    {
                        var surahs = new System.Text.StringBuilder();
                        foreach (var row in byDate[d])
                        {
                            string planTypeDisplay = row.LatestStatus == StatusPass ? "تثبيت" : row.PlanType;
                    string typeClass = planTypeDisplay == "حفظ" ? "cal-type-mem" : (planTypeDisplay == "مراجعة" ? "cal-type-rev" : "cal-type-tathbit");
                    string quranLinkUrl = VirtualPathUtility.ToAbsolute("~/Quran.aspx") + "?surahId=" + row.SurahId
                        + "&from=" + row.FromAyahNumber + "&to=" + row.ToAyahNumber;
                    surahs.AppendFormat("<div class='cal-surah-item'><span class='cal-plan-type {0}'>{1}</span> {2} <a href='{3}' target='_blank' class='cal-quran-icon' title='فتح في المصحف'><i class='fas fa-book-quran'></i></a></div>",
                        typeClass, System.Web.HttpUtility.HtmlEncode(planTypeDisplay), System.Web.HttpUtility.HtmlEncode("سورة " + row.SurahName), quranLinkUrl);
                        }
                        vm.ItemsHtml = surahs.ToString();
                    }

                    calDays.Add(vm);
                }

                CalendarRepeater.DataSource = calDays;
                CalendarRepeater.DataBind();
            }
            catch { CalendarRepeater.DataSource = null; CalendarRepeater.DataBind(); }
        }

        private void SetDaysRemainingAndProgress(int planId)
        {
            var memRows = db.StudentPlanMemorizings.Where(x => x.PlanId == planId).Select(x => new { x.PlanDate, x.PlanEndDate, x.MemorizationLevel }).ToList();
            var revRows = db.StudentPlanRevises.Where(x => x.PlanId == planId).Select(x => new { x.PlanDate, x.PlanEndDate, x.MemorizationLevel }).ToList();
            var all = memRows.Concat(revRows).ToList();
            if (all.Count == 0) return;

            var student = db.RegisterForms.FirstOrDefault(x => x.Id == (ViewStudentId ?? 0));
            List<int> circleDayNumbers = new List<int>();
            if (student != null && student.QuranCircleId.HasValue)
            {
                circleDayNumbers = db.CircleDays
                    .Where(d => d.CircleId == student.QuranCircleId.Value)
                    .Select(d => d.DayNumber)
                    .ToList();
            }

            var planStart = all.Min(x => x.PlanDate);
            var planEnd = all.Max(x => x.PlanEndDate ?? x.PlanDate);
            var today = KuwaitTime.Today;

            int totalPlanDays = CalcCircleDaysInList(planStart.Date, planEnd.Date, circleDayNumbers);
            if (totalPlanDays <= 0) totalPlanDays = Math.Max(1, (planEnd.Date - planStart.Date).Days + 1);

            int daysRemaining = CalcCircleDaysInList(today > planStart.Date ? today : planStart.Date, planEnd.Date, circleDayNumbers);
            if (today > planEnd.Date) daysRemaining = 0;

            int daysElapsed = totalPlanDays - daysRemaining;
            if (daysElapsed < 0) daysElapsed = 0;


            var memStatuses = db.StudentPlanMemorizings.Where(x => x.PlanId == planId).Select(x => x.Status).ToList();
            var revStatuses = db.StudentPlanRevises.Where(x => x.PlanId == planId).Select(x => x.Status).ToList();
            var allStatuses = memStatuses.Concat(revStatuses).ToList();
            int passed = allStatuses.Count(s => s == StatusPass);
            int failed = allStatuses.Count(s => s == StatusFail);
            int retake = allStatuses.Count(s => s == StatusRetake);
            int pending = Math.Max(0, allStatuses.Count - passed - failed - retake);
            int total = allStatuses.Count;
            int progressPercent = total > 0 ? (int)Math.Round(100.0 * passed / total) : 0;

            if (LiteralProgressData != null)
            {
                LiteralProgressData.Mode = LiteralMode.PassThrough;
                LiteralProgressData.Text = string.Format("{{\"passed\":{0},\"failed\":{1},\"pending\":{2},\"retake\":{3},\"total\":{4},\"daysRemaining\":{5},\"totalPlanDays\":{6},\"daysElapsed\":{7},\"progressPercent\":{8}}}",
                    passed, failed, pending, retake, total, daysRemaining, totalPlanDays, daysElapsed, progressPercent);
            }

           

            // Plan info section: level, from-to date, total circle days (only when we have plan data)
            if (LiteralPlanInfo != null && all.Count > 0)
            {
                string level = (all.OrderByDescending(x => x.PlanDate).FirstOrDefault())?.MemorizationLevel;
                int circleDays = ViewStudentId.HasValue ? GetCircleDaysCount(ViewStudentId.Value, planStart, planEnd) : 0;
                LiteralPlanInfo.Text = string.Format(
                    "<div style='display:flex;flex-wrap:wrap;gap:15px 25px;'><span><strong>المستوى:</strong> {0}</span><span><strong>من:</strong> {1} <strong>إلى:</strong> {2}</span><span><strong>إجمالي أيام الخطة (أيام الحلقة):</strong> {3} يوم</span></div>",
                    string.IsNullOrEmpty(level) ? "—" : level,
                    planStart.ToString("yyyy-MM-dd"),
                    planEnd.ToString("yyyy-MM-dd"),
                    circleDays
                );
            }
        }

        private int GetCircleDaysCount(int studentId, DateTime start, DateTime end)
        {
            var student = db.RegisterForms.FirstOrDefault(x => x.Id == studentId);
            if (student == null || !student.QuranCircleId.HasValue) return 0;

            var circleDayNumbers = db.CircleDays
                .Where(d => d.CircleId == student.QuranCircleId.Value)
                .Select(d => d.DayNumber)
                .ToList();

            return CalcCircleDaysInList(start, end, circleDayNumbers);
        }

        private int CalcCircleDaysInList(DateTime start, DateTime end, List<int> circleDayNumbers)
        {
            if (circleDayNumbers == null || circleDayNumbers.Count == 0) return 0;
            int count = 0;
            for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
            {
                if (circleDayNumbers.Contains((int)d.DayOfWeek))
                    count++;
            }
            return count;
        }

        private DateTime GetNextCircleDayForLogic(DateTime current, List<int> circleDays)
        {
            if (circleDays == null || !circleDays.Any()) return current.AddDays(1);
            DateTime next = current.AddDays(1);
            int safety = 0;
            while (!circleDays.Contains((int)next.DayOfWeek) && safety < 30)
            {
                next = next.AddDays(1);
                safety++;
            }
            return next;
        }

        private static DateTime GetNextCircleDayForLogicStatic(DateTime current, List<int> circleDays)
        {
            if (circleDays == null || !circleDays.Any()) return current.AddDays(1);
            DateTime next = current.AddDays(1);
            int safety = 0;
            while (!circleDays.Contains((int)next.DayOfWeek) && safety < 30)
            {
                next = next.AddDays(1);
                safety++;
            }
            return next;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object AssignReviseToExistingPlan(int studentId, int planId, int planLevelId, int fromSurahId, int toSurahId, int? fromJozz, int? toJozz, string fromDate, string toDate, int? fromAyahNumber, int? toAyahNumber)
        {
            try
            {
                using (var db = new Model1())
                {
                    if (!CurrentTeacherOwnsStudent(db, studentId))
                        return new { success = false, error = "غير مصرح" };

                    var plan = db.StudentPlans.FirstOrDefault(p => p.Id == planId && p.StudentId == studentId);
                    if (plan == null) return new { success = false, error = "الخطة غير موجودة" };

                    var level = db.PlanLevels.FirstOrDefault(l => l.Id == planLevelId);
                    if (level == null) return new { success = false, error = "مستوى الخطة غير موجود" };

                    DateTime startDate = DateTime.TryParse(fromDate, out var fd) ? fd.Date : KuwaitTime.Today;
                    DateTime endDate = DateTime.TryParse(toDate, out var td) ? td.Date : KuwaitTime.Today;
                    if (endDate < startDate) endDate = startDate;

                    var unit = (PlanUnitType)level.UnitType;
                    int dailyQty = Math.Max(1, level.Quantity);

                    var userAcc = new TheUser();
                    int circleId = userAcc.CircleId();
                    var circleDayNumbers = db.CircleDays
                        .Where(d => d.CircleId == circleId)
                        .Select(d => d.DayNumber)
                        .ToList();

                    List<Holyquran> allQuranData;

                    if (unit == PlanUnitType.Jozz && fromJozz.HasValue && toJozz.HasValue)
                    {
                        int minJ = Math.Min(fromJozz.Value, toJozz.Value);
                        int maxJ = Math.Max(fromJozz.Value, toJozz.Value);
                        allQuranData = db.Holyqurans
                            .Where(h => h.jozz >= minJ && h.jozz <= maxJ)
                            .OrderBy(h => h.jozz).ThenBy(h => h.page).ThenBy(h => h.aya_no)
                            .ToList();
                    }
                    else
                    {
                        var allSurahs = db.QuranSurahs.OrderBy(x => x.SortOrder ?? x.Id).ToList();
                        var fromS = allSurahs.FirstOrDefault(s => s.Id == fromSurahId);
                        var toS = allSurahs.FirstOrDefault(s => s.Id == toSurahId);
                        if (fromS == null || toS == null) return new { success = false, error = "سورة غير صالحة" };
                        int minOrder = Math.Min(fromS.SortOrder ?? fromS.Id, toS.SortOrder ?? toS.Id);
                        int maxOrder = Math.Max(fromS.SortOrder ?? fromS.Id, toS.SortOrder ?? toS.Id);
                        var targetSurahIds = allSurahs
                            .Where(s => (s.SortOrder ?? s.Id) >= minOrder && (s.SortOrder ?? s.Id) <= maxOrder)
                            .Select(s => s.Id).ToList();

                        allQuranData = db.Holyqurans
                            .Where(h => targetSurahIds.Contains(h.sura_no))
                            .OrderBy(h => h.sura_no).ThenBy(h => h.aya_no)
                            .ToList();

                        if (fromAyahNumber.HasValue || toAyahNumber.HasValue)
                        {
                            allQuranData = allQuranData.Where(h =>
                            {
                                if (h.sura_no == fromSurahId && fromAyahNumber.HasValue && h.aya_no < fromAyahNumber.Value) return false;
                                if (h.sura_no == toSurahId && toAyahNumber.HasValue && h.aya_no > toAyahNumber.Value) return false;
                                return true;
                            }).ToList();
                        }
                    }

                    if (!allQuranData.Any()) return new { success = false, error = "لا توجد بيانات للقرآن في النطاق المحدد" };

                    var rowsWithDates = new List<Tuple<int, int, int, DateTime>>();
                    var surahIdsInOrder = allQuranData.Select(h => h.sura_no).Distinct().ToList();

                    DateTime currentDate = startDate;
                    foreach (int surahId in surahIdsInOrder)
                    {
                        var surahData = allQuranData.Where(h => h.sura_no == surahId)
                            .OrderBy(h => h.page).ThenBy(h => h.aya_no).ToList();

                        switch (unit)
                        {
                            case PlanUnitType.Page:
                            case PlanUnitType.QuarterPage:
                            case PlanUnitType.Line:
                                {
                                    int targetLines = dailyQty;
                                    if (unit == PlanUnitType.Page) targetLines = 15 * dailyQty;
                                    else if (unit == PlanUnitType.QuarterPage) targetLines = 4 * dailyQty;
                                    int i = 0;
                                    while (i < surahData.Count && currentDate <= endDate)
                                    {
                                        int startAya = surahData[i].aya_no;
                                        int currentAya = startAya;
                                        int acc = 0;
                                        while (i < surahData.Count && acc < targetLines)
                                        {
                                            var aya = surahData[i];
                                            int linesInAya = (aya.line_end ?? 1) - (aya.line_start ?? 1) + 1;
                                            if (linesInAya <= 0) linesInAya = 1;
                                            acc += linesInAya;
                                            currentAya = aya.aya_no;
                                            i++;
                                        }
                                        rowsWithDates.Add(Tuple.Create(surahId, startAya, currentAya, currentDate));
                                        currentDate = GetNextCircleDayForLogicStatic(currentDate, circleDayNumbers);
                                    }
                                }
                                break;
                            case PlanUnitType.Jozz:
                                {
                                    var surahJozzs = surahData.GroupBy(h => h.jozz).Select(g => new
                                    {
                                        Jozz = g.Key,
                                        MinAya = g.Min(x => x.aya_no),
                                        MaxAya = g.Max(x => x.aya_no)
                                    }).OrderBy(x => x.Jozz).ToList();

                                    foreach (var jPart in surahJozzs)
                                    {
                                        if (currentDate <= endDate)
                                        {
                                            rowsWithDates.Add(Tuple.Create(surahId, jPart.MinAya, jPart.MaxAya, currentDate));
                                            currentDate = GetNextCircleDayForLogicStatic(currentDate, circleDayNumbers);
                                        }
                                    }
                                }
                                break;
                        }
                    }

                    foreach (var row in rowsWithDates)
                    {
                        db.StudentPlanRevises.Add(new StudentPlanRevise
                        {
                            StudentId = studentId,
                            PlanId = planId,
                            MemorizationLevel = level.LevelName,
                            SurahId = row.Item1,
                            FromAyahNumber = row.Item2,
                            ToAyahNumber = row.Item3,
                            PlanDate = row.Item4,
                            PlanEndDate = row.Item4,
                            CreatedAt = KuwaitTime.Now
                        });
                    }
                    db.SaveChanges();
                    return new { success = true };
                }
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object GetCircleDaysCount(string startDateStr, string endDateStr)
        {
            DateTime startDate, endDate;
            if (!DateTime.TryParse(startDateStr, out startDate) || !DateTime.TryParse(endDateStr, out endDate))
            {
                return new { success = false, error = "Invalid dates" };
            }
            if (endDate < startDate) endDate = startDate;
            try
            {
                using (var db = new Model1())
                {
                    var user = new TheUser();
                    int circleId = user.CircleId();
                    var dayNumbers = db.CircleDays
                        .Where(d => d.CircleId == circleId)
                        .Select(d => d.DayNumber)
                        .ToList();
                    int count = 0;
                    for (var d = startDate; d <= endDate; d = d.AddDays(1))
                    {
                        if (dayNumbers.Contains((int)d.DayOfWeek))
                            count++;
                    }
                    return new { success = true, count = count };
                }
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        protected void BtnCalcCircleDays_Click(object sender, EventArgs e)
        {
            if (ViewStudentId.HasValue && ViewPlanId.HasValue)
                LoadPlansForStudent(ViewStudentId.Value, ViewPlanId.Value);
        }

        private void LoadAssessmentLog(int planId)
        {
            var logEntries = db.StudentPlanItemLogs.Where(x => x.PlanId == planId)
                .Include(x => x.Teacher)
                .OrderByDescending(x => x.LoggedAt)
                .Take(50)
                .Select(x => new { x.RowKey, x.Status, x.LoggedAt, TeacherName = x.Teacher != null ? x.Teacher.Name : "" })
                .ToList();
            var keys = logEntries.Select(x => x.RowKey).Distinct().ToList();
            var rowLabels = new Dictionary<string, string>();
            foreach (var key in keys)
            {
                string label = key;
                if (key.StartsWith("memorizing_") && int.TryParse(key.Substring("memorizing_".Length), out int memId))
                {
                    var e = db.StudentPlanMemorizings.Include(x => x.QuranSurah).FirstOrDefault(x => x.Id == memId && x.PlanId == planId);
                    if (e != null) label = (e.QuranSurah?.NameAr ?? "—") + " " + e.FromAyahNumber + "-" + e.ToAyahNumber;
                }
                else if (key.StartsWith("revise_") && int.TryParse(key.Substring("revise_".Length), out int revId))
                {
                    var e = db.StudentPlanRevises.Include(x => x.QuranSurah).FirstOrDefault(x => x.Id == revId && x.PlanId == planId);
                    if (e != null) label = (e.QuranSurah?.NameAr ?? "—") + " " + e.FromAyahNumber + "-" + e.ToAyahNumber;
                }
                rowLabels[key] = label;
            }
            var list = logEntries.Select(x =>
            {
                string statusCss = "pending";
                if (x.Status == StatusPass) statusCss = "pass";
                else if (x.Status == StatusFail) statusCss = "fail";
                else if (x.Status == StatusRetake) statusCss = "retake";
                string statusDisplay = GetStatusDisplayLabel(x.RowKey, x.Status);
                return new { RowLabel = rowLabels.ContainsKey(x.RowKey) ? rowLabels[x.RowKey] : x.RowKey, x.Status, StatusDisplay = statusDisplay, StatusCss = statusCss, x.TeacherName, LoggedAtFormatted = x.LoggedAt.ToString("yyyy-MM-dd hh:mm tt") };
            }).ToList();
            RepeaterAssessmentLog.DataSource = list;
            RepeaterAssessmentLog.DataBind();
            if (PanelAssessmentLogEmpty != null)
                PanelAssessmentLogEmpty.Visible = list.Count == 0;
        }

        protected void PlansRepeaterEdit_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;
            var key = DataBinder.Eval(e.Item.DataItem, "Key")?.ToString();
            var surahIdObj = DataBinder.Eval(e.Item.DataItem, "SurahId");
            int surahId = surahIdObj != null && surahIdObj != DBNull.Value ? Convert.ToInt32(surahIdObj) : 0;
            var fromObj = DataBinder.Eval(e.Item.DataItem, "FromAyahNumber");
            int fromAyah = fromObj != null && fromObj != DBNull.Value ? Convert.ToInt32(fromObj) : 0;
            var toObj = DataBinder.Eval(e.Item.DataItem, "ToAyahNumber");
            int toAyah = toObj != null && toObj != DBNull.Value ? Convert.ToInt32(toObj) : 0;
            var planType = DataBinder.Eval(e.Item.DataItem, "PlanType")?.ToString() ?? "حفظ";

            var surahName = DataBinder.Eval(e.Item.DataItem, "SurahName")?.ToString() ?? "—";

            var surahDdl = (DropDownList)e.Item.FindControl("SurahDropDown");
            if (surahDdl != null)
            {
                var surahs = db.QuranSurahs.OrderBy(x => x.Id).Select(x => new { x.Id, x.NameAr }).ToList();
                surahDdl.DataSource = surahs;
                surahDdl.DataValueField = "Id";
                surahDdl.DataTextField = "NameAr";
                surahDdl.DataBind();
                if (surahId > 0)
                    surahDdl.SelectedValue = surahId.ToString();
            }
            var surahLabel = (HtmlGenericControl)e.Item.FindControl("SurahLabelSpan");
            if (surahLabel != null) surahLabel.InnerText = surahName ?? "—";

            var fromDdl = (DropDownList)e.Item.FindControl("FromAyahDropDown");
            var toDdl = (DropDownList)e.Item.FindControl("ToAyahDropDown");
            if (fromDdl != null && toDdl != null && surahId > 0)
            {
                var ayahs = db.QuranAyahs.Where(x => x.SurahId == surahId).OrderBy(x => x.AyahNumber).Select(x => x.AyahNumber).ToList();
                fromDdl.Items.Clear();
                toDdl.Items.Clear();
                foreach (int a in ayahs)
                {
                    fromDdl.Items.Add(new ListItem(a.ToString(), a.ToString()));
                    toDdl.Items.Add(new ListItem(a.ToString(), a.ToString()));
                }
                if (fromAyah > 0) fromDdl.SelectedValue = fromAyah.ToString();
                if (toAyah > 0) toDdl.SelectedValue = toAyah.ToString();
            }
            var fromLabel = (HtmlGenericControl)e.Item.FindControl("FromAyahLabelSpan");
            if (fromLabel != null) fromLabel.InnerText = fromAyah > 0 ? fromAyah.ToString() : "--";
            var toLabel = (HtmlGenericControl)e.Item.FindControl("ToAyahLabelSpan");
            if (toLabel != null) toLabel.InnerText = toAyah > 0 ? toAyah.ToString() : "--";

            var typeDdl = (DropDownList)e.Item.FindControl("TypeDropDown");
            if (typeDdl != null && !string.IsNullOrEmpty(planType))
                typeDdl.SelectedValue = planType;
            var typeLabel = (HtmlGenericControl)e.Item.FindControl("TypeLabelSpan");
            if (typeLabel != null) typeLabel.InnerText = planType ?? "حفظ";
        }

        protected void PlansRepeaterEdit_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string key = e.CommandArgument?.ToString();
            if (string.IsNullOrEmpty(key)) return;
            if (e.CommandName == "Delete")
            {
                if (key.StartsWith("memorizing_") && int.TryParse(key.Substring("memorizing_".Length), out int memId))
                {
                    var ent = db.StudentPlanMemorizings.Find(memId);
                    if (ent != null) { db.StudentPlanMemorizings.Remove(ent); db.SaveChanges(); }
                }
                else if (key.StartsWith("revise_") && int.TryParse(key.Substring("revise_".Length), out int revId))
                {
                    var ent = db.StudentPlanRevises.Find(revId);
                    if (ent != null) { db.StudentPlanRevises.Remove(ent); db.SaveChanges(); }
                }
                if (ViewStudentId.HasValue && ViewPlanId.HasValue)
                    LoadPlansForStudent(ViewStudentId.Value, ViewPlanId.Value);
            }
            else if (e.CommandName == "OpenQuran")
            {
                OpenQuranForPlanItem(key);
            }
        }

        protected void PlansRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string key = e.CommandArgument?.ToString();
            if (string.IsNullOrEmpty(key)) return;

            if (e.CommandName == "Delete")
            {
                if (key.StartsWith("memorizing_") && int.TryParse(key.Substring("memorizing_".Length), out int memId))
                {
                    var ent = db.StudentPlanMemorizings.Find(memId);
                    if (ent != null) { db.StudentPlanMemorizings.Remove(ent); db.SaveChanges(); }
                }
                else if (key.StartsWith("revise_") && int.TryParse(key.Substring("revise_".Length), out int revId))
                {
                    var ent = db.StudentPlanRevises.Find(revId);
                    if (ent != null) { db.StudentPlanRevises.Remove(ent); db.SaveChanges(); }
                }
                if (ViewStudentId.HasValue && ViewPlanId.HasValue)
                    LoadPlansForStudent(ViewStudentId.Value, ViewPlanId.Value);
                return;
            }

            if (e.CommandName == "Edit")
            {
                string qs = "studentId=" + ViewStudentId + "&edit=" + System.Web.HttpUtility.UrlEncode(key);
                if (ViewPlanId.HasValue) qs += "&planId=" + ViewPlanId.Value;
                Response.Redirect("StudentPlan2.aspx?" + qs);
            }

            if (e.CommandName == "OpenQuran")
            {
                OpenQuranForPlanItem(key);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object GetAyahsBySurah(int surahId)
        {
            using (var db = new Model1())
            {
                var list = db.QuranAyahs.Where(x => x.SurahId == surahId).OrderBy(x => x.AyahNumber).Select(x => new { x.AyahNumber }).ToList();
                return list;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object LogPlanRowStatus(string rowKey, string status, string tabType)
        {
            var ctx = HttpContext.Current;
            if (ctx == null) return new { success = false };
            string studentIdStr = ctx.Request.QueryString["studentId"];
            if (string.IsNullOrEmpty(studentIdStr) || !int.TryParse(studentIdStr, out int studentId))
                return new { success = false };

            using (var dbGate = new Model1())
            {
                if (!CurrentTeacherOwnsStudent(dbGate, studentId)) return new { success = false };
            }

            var validStatuses = new[] { StatusPass, StatusFail, StatusPending, StatusRetake, StatusPendingTathbit };
            if (string.IsNullOrEmpty(status) || !validStatuses.Contains(status))
                return new { success = false };

            // Logic for transition to Tathbit: Only from 'Memorizing' tab
            if (status == StatusPass && tabType == "حفظ")
            {
                status = StatusPendingTathbit;
            }
            int planId = 0;
            if (rowKey.StartsWith("memorizing_") && int.TryParse(rowKey.Substring("memorizing_".Length), out int memId))
            {
                using (var db = new Model1())
                {
                    var ent = db.StudentPlanMemorizings.FirstOrDefault(x => x.Id == memId && x.StudentId == studentId);
                    if (ent == null) return new { success = false };
                    planId = ent.PlanId;
                }
            }
            else if (rowKey.StartsWith("revise_") && int.TryParse(rowKey.Substring("revise_".Length), out int revId))
            {
                using (var db = new Model1())
                {
                    var ent = db.StudentPlanRevises.FirstOrDefault(x => x.Id == revId && x.StudentId == studentId);
                    if (ent == null) return new { success = false };
                    planId = ent.PlanId;
                }
            }
            else
                return new { success = false };
            int teacherId = new TheUser().Id();
            if (teacherId <= 0) return new { success = false };
            using (var db = new Model1())
            {
                var loggedAt = KuwaitTime.Now;
                db.StudentPlanItemLogs.Add(new StudentPlanItemLog
                {
                    StudentId = studentId,
                    PlanId = planId,
                    RowKey = rowKey,
                    Status = status,
                    TeacherId = teacherId,
                    LoggedAt = loggedAt
                });
                if (rowKey.StartsWith("memorizing_") && int.TryParse(rowKey.Substring("memorizing_".Length), out int memIdUpdate))
                {
                    var ent = db.StudentPlanMemorizings.Find(memIdUpdate);
                    if (ent != null) ent.Status = status;
                }
                else if (rowKey.StartsWith("revise_") && int.TryParse(rowKey.Substring("revise_".Length), out int revIdUpdate))
                {
                    var ent = db.StudentPlanRevises.Find(revIdUpdate);
                    if (ent != null) ent.Status = status;
                }
                db.SaveChanges();
                var memStatuses = db.StudentPlanMemorizings.Where(x => x.PlanId == planId).Select(x => x.Status).ToList();
                var revStatuses = db.StudentPlanRevises.Where(x => x.PlanId == planId).Select(x => x.Status).ToList();
                var allStatuses = memStatuses.Concat(revStatuses).ToList();
                int passed = allStatuses.Count(s => s == StatusPass);
                int failed = allStatuses.Count(s => s == StatusFail);
                int retake = allStatuses.Count(s => s == StatusRetake);
                int pending = Math.Max(0, allStatuses.Count - passed - failed - retake);
                string rowLabel = rowKey;
                if (rowKey.StartsWith("memorizing_") && int.TryParse(rowKey.Substring("memorizing_".Length), out int memIdForLabel))
                {
                    var e = db.StudentPlanMemorizings.Include(x => x.QuranSurah).FirstOrDefault(x => x.Id == memIdForLabel);
                    if (e != null) rowLabel = "حفظ: " + (e.QuranSurah?.NameAr ?? "—") + " " + e.FromAyahNumber + "-" + e.ToAyahNumber;
                }
                else if (rowKey.StartsWith("revise_") && int.TryParse(rowKey.Substring("revise_".Length), out int revIdForLabel))
                {
                    var e = db.StudentPlanRevises.Include(x => x.QuranSurah).FirstOrDefault(x => x.Id == revIdForLabel);
                    if (e != null) rowLabel = "مراجعة: " + (e.QuranSurah?.NameAr ?? "—") + " " + e.FromAyahNumber + "-" + e.ToAyahNumber;
                }
                string statusDisplay = GetStatusDisplayLabel(rowKey, status);
                string teacherName = db.Teachers.Where(x => x.Id == teacherId).Select(x => x.Name).FirstOrDefault() ?? "";
                string loggedAtFormatted = loggedAt.ToString("yyyy-MM-dd hh:mm tt");
                var logRow = new { rowLabel, statusDisplay, teacherName, loggedAtFormatted };

                // When revise row status changed to تم/لم يتم/اعادة تسميع, return next pending revise record
                object nextReviseRecord = null;
                var reviseStatuses = new[] { StatusPass, StatusFail, StatusRetake };
                if (rowKey.StartsWith("revise_") && reviseStatuses.Contains(status))
                {
                    var nextRev = db.StudentPlanRevises
                        .Where(x => x.StudentId == studentId && x.PlanId == planId && x.Status == StatusPending)
                        .Include(x => x.QuranSurah)
                        .OrderBy(x => x.PlanDate)
                        .FirstOrDefault();
                    if (nextRev != null)
                        nextReviseRecord = new { key = "revise_" + nextRev.Id, surahId = nextRev.SurahId, surahName = nextRev.QuranSurah?.NameAr ?? "—", fromAyah = nextRev.FromAyahNumber, toAyah = nextRev.ToAyahNumber };
                }

                return new { success = true, passed, failed, pending, retake, total = allStatuses.Count, logRow, nextReviseRecord };
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object GetPlanProgress(int studentId, int planId)
        {
            using (var db = new Model1())
            {
                if (!CurrentTeacherOwnsStudent(db, studentId))
                    return new { passed = 0, failed = 0, pending = 0, retake = 0, total = 0 };

                var memStatuses = db.StudentPlanMemorizings.Where(x => x.StudentId == studentId && x.PlanId == planId).Select(x => x.Status).ToList();
                var revStatuses = db.StudentPlanRevises.Where(x => x.StudentId == studentId && x.PlanId == planId).Select(x => x.Status).ToList();
                var allStatuses = memStatuses.Concat(revStatuses).ToList();
                if (allStatuses.Count == 0) return new { passed = 0, failed = 0, pending = 0, retake = 0, total = 0 };
                int passed = allStatuses.Count(s => s == StatusPass);
                int failed = allStatuses.Count(s => s == StatusFail);
                int retake = allStatuses.Count(s => s == StatusRetake);
                int pending = Math.Max(0, allStatuses.Count - passed - failed - retake);
                return new { passed, failed, pending, retake, total = allStatuses.Count };
            }
        }

        private void OpenQuranForPlanItem(string key)
        {
            int surahId = 0;
            int fromAyah = 0;
            int toAyah = 0;

            if (key.StartsWith("memorizing_") && int.TryParse(key.Substring("memorizing_".Length), out int memId))
            {
                var ent = db.StudentPlanMemorizings.Include(x => x.QuranSurah).FirstOrDefault(x => x.Id == memId);
                if (ent != null)
                {
                    surahId = ent.SurahId;
                    fromAyah = ent.FromAyahNumber;
                    toAyah = ent.ToAyahNumber;
                }
            }
            else if (key.StartsWith("revise_") && int.TryParse(key.Substring("revise_".Length), out int revId))
            {
                var ent = db.StudentPlanRevises.Include(x => x.QuranSurah).FirstOrDefault(x => x.Id == revId);
                if (ent != null)
                {
                    surahId = ent.SurahId;
                    fromAyah = ent.FromAyahNumber;
                    toAyah = ent.ToAyahNumber;
                }
            }

            if (surahId > 0 && fromAyah > 0)
            {
                // Find the page number for the first ayah using direct SQL query
                int page = 1;
                bool pageFound = false;
                using (var con = new System.Data.SqlClient.SqlConnection(masabihq8Shared.ConnectionStrings.Production))
                {
                    con.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand(
                        "SELECT TOP 1 [page] FROM [dbo].[HolyQuran] WHERE [sura_no] = @surahId AND [aya_no] = @ayahNumber ORDER BY [line_start]", con))
                    {
                        cmd.Parameters.AddWithValue("@surahId", surahId);
                        cmd.Parameters.AddWithValue("@ayahNumber", fromAyah);
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            page = Convert.ToInt32(result);
                            pageFound = true;
                        }
                    }
                }

                if (pageFound)
                {
                    // Open Quran page with the specific surah and ayah
                    // Note: If ayah range spans multiple pages, user may need to navigate to see all ayahs
                    string quranUrl = $"Quran.aspx?page={page}&surah={surahId}&fromAyah={fromAyah}&toAyah={toAyah}";

                    // Open in new window using JavaScript
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenQuran",
                        $"window.open('{quranUrl}', '_blank');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "QuranError",
                        $"alert('لم يتم العثور على الآية {fromAyah} في السورة {surahId} في قاعدة البيانات.');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "QuranError",
                    "alert('بيانات السورة أو الآية غير متوفرة.');", true);
            }
        }

        protected string GetDateDisplay(object dateValue)
        {
            if (dateValue == null || dateValue == DBNull.Value)
                return "—";

            if (dateValue is DateTime dateTime && dateTime != DateTime.MinValue)
                return dateTime.ToString("yyyy-MM-dd");

            return "—";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object SaveNextMemorizeDate(List<string> itemKeys, string date)
        {
            var ctx = HttpContext.Current;
            if (ctx == null) return new { success = false, message = "No HTTP context" };

            string studentIdStr = ctx.Request.QueryString["studentId"];
            if (string.IsNullOrEmpty(studentIdStr) || !int.TryParse(studentIdStr, out int studentId))
                return new { success = false, message = "Invalid student ID" };

            if (itemKeys == null || itemKeys.Count == 0)
                return new { success = false, message = "No items selected" };

            if (string.IsNullOrEmpty(date) || !DateTime.TryParse(date, out DateTime selectedDate))
                return new { success = false, message = "Invalid date" };

            try
            {
                using (var db = new Model1())
                {
                    if (!CurrentTeacherOwnsStudent(db, studentId))
                        return new { success = false, message = "غير مصرح" };

                    foreach (var itemKey in itemKeys)
                    {
                        if (itemKey.StartsWith("memorizing_") && int.TryParse(itemKey.Substring("memorizing_".Length), out int memId))
                        {
                            var ent = db.StudentPlanMemorizings.FirstOrDefault(x => x.Id == memId && x.StudentId == studentId);
                            if (ent != null)
                            {
                                // For memorizing items, set MemorizeDate
                                ent.MemorizeDate = selectedDate;
                            }
                        }
                        else if (itemKey.StartsWith("revise_") && int.TryParse(itemKey.Substring("revise_".Length), out int revId))
                        {
                            var ent = db.StudentPlanRevises.FirstOrDefault(x => x.Id == revId && x.StudentId == studentId);
                            if (ent != null)
                            {
                                // For revise items, set ReviseDate
                                ent.ReviseDate = selectedDate;
                            }
                        }
                    }

                    db.SaveChanges();
                    return new { success = true, message = "تم حفظ التاريخ بنجاح" };
                }
            }
            catch (Exception ex)
            {
                return new { success = false, message = "خطأ: " + ex.Message };
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object SaveNextReviseDate(List<string> itemKeys, string date)
        {
            var ctx = HttpContext.Current;
            if (ctx == null) return new { success = false, message = "No HTTP context" };

            string studentIdStr = ctx.Request.QueryString["studentId"];
            if (string.IsNullOrEmpty(studentIdStr) || !int.TryParse(studentIdStr, out int studentId))
                return new { success = false, message = "Invalid student ID" };

            if (itemKeys == null || itemKeys.Count == 0)
                return new { success = false, message = "No items selected" };

            if (string.IsNullOrEmpty(date) || !DateTime.TryParse(date, out DateTime selectedDate))
                return new { success = false, message = "Invalid date" };

            try
            {
                using (var db = new Model1())
                {
                    if (!CurrentTeacherOwnsStudent(db, studentId))
                        return new { success = false, message = "غير مصرح" };

                    foreach (var itemKey in itemKeys)
                    {
                        if (itemKey.StartsWith("memorizing_") && int.TryParse(itemKey.Substring("memorizing_".Length), out int memId))
                        {
                            var ent = db.StudentPlanMemorizings.FirstOrDefault(x => x.Id == memId && x.StudentId == studentId);
                            if (ent != null)
                            {
                                ent.ReviseDate = selectedDate;
                            }
                        }
                        else if (itemKey.StartsWith("revise_") && int.TryParse(itemKey.Substring("revise_".Length), out int revId))
                        {
                            var ent = db.StudentPlanRevises.FirstOrDefault(x => x.Id == revId && x.StudentId == studentId);
                            if (ent != null)
                            {
                                ent.ReviseDate = selectedDate;
                            }
                        }
                    }

                    db.SaveChanges();
                    return new { success = true, message = "تم حفظ التاريخ بنجاح" };
                }
            }
            catch (Exception ex)
            {
                return new { success = false, message = "خطأ: " + ex.Message };
            }
        }
    }
}

