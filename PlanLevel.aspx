<%@ Page Title="مستويات الخطة (خاص بالمعلم)" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="PlanLevel.aspx.cs" Inherits="masabihq8Teacher.PlanLevel" EnableEventValidation="false" %>


    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <style>
            .form-card {
                background: #fff;
                padding: 20px;
                border: 1px solid #eee;
                border-radius: 8px;
                margin-bottom: 20px
            }

            .grid-card {
                background: #fff;
                padding: 20px;
                border: 1px solid #eee;
                border-radius: 8px
            }

            .form-row {
                display: flex;
                gap: 15px;
                align-items: flex-end;
                flex-wrap: wrap
            }

            .form-group {
                min-width: 220px
            }

            .form-label {
                display: block;
                margin-bottom: 6px;
                font-weight: 600
            }

            .form-control {
                width: 100%;
                padding: 8px 10px;
                border: 1px solid #ddd;
                border-radius: 6px
            }

            .btn {
                padding: 8px 14px;
                border: none;
                border-radius: 6px;
                cursor: pointer
            }

            .btn-primary {
                background: #1e88e5;
                color: #fff
            }

            .btn-danger {
                background: #dc3545;
                color: #fff
            }

            .btn-secondary {
                background: #6c757d;
                color: #fff
            }

            .page-header {
                background: linear-gradient(135deg, #7C8738, #1a5f8a);
                color: #fff;
                padding: 30px;
                border-radius: 12px;
                margin-bottom: 30px;
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1)
            }

            .page-title {
                font-size: 2rem;
                font-weight: 700;
                margin: 0;
                text-align: center
            }

            .page-subtitle {
                font-size: 1rem;
                opacity: .9;
                text-align: center;
                margin-top: 10px
            }

            .text-center {
                text-align: center
            }

            .badge-private {
                background: #6f42c1;
                color: #fff;
                padding: 2px 6px;
                border-radius: 4px;
                font-size: 0.8rem;
            }

            .badge-global {
                background: #28a745;
                color: #fff;
                padding: 2px 6px;
                border-radius: 4px;
                font-size: 0.8rem;
            }
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="page-header">
            <div class="page-title">مستويات الخطة والخطط الجاهزة</div>
            <div class="page-subtitle">قم بإنشاء خطط خاصة بك لاستخدامها مع طلابك</div>
        </div>

        <div class="form-card">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="pg" CssClass="text-danger" />
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">اسم المستوى</label>
                    <asp:TextBox ID="LevelNameTb" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="LevelNameTb"
                        ErrorMessage="الاسم مطلوب" CssClass="text-danger" ValidationGroup="pg" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label class="form-label">نوع القدرة</label>
                    <asp:DropDownList ID="UnitTypeDdl" runat="server" CssClass="form-control"
                        onchange="toggleInputs();">
                        <asp:ListItem Value="0" Text="صفحة" />
                        <asp:ListItem Value="1" Text="ربع" />
                        <asp:ListItem Value="3" Text="سطر" />
                    </asp:DropDownList>
                </div>
                <div class="form-group">
                    <label class="form-label">الكمية</label>
                    <asp:TextBox ID="QuantityTb" runat="server" CssClass="form-control" TextMode="Number" />
                    <asp:RequiredFieldValidator ID="rfvQty" runat="server" ControlToValidate="QuantityTb"
                        ErrorMessage="الكمية مطلوبة" CssClass="text-danger" ValidationGroup="pg" Display="Dynamic" />
                </div>
                <div class="form-group range-surah">
                    <label class="form-label">من سورة</label>
                    <asp:DropDownList ID="FromSurahDropDown" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group range-ayah" style="display:none;">
                    <label class="form-label">من آية</label>
                    <asp:TextBox ID="FromAyahTb" runat="server" CssClass="form-control" TextMode="Number" />
                </div>
                <div class="form-group range-surah">
                    <label class="form-label">إلى سورة</label>
                    <asp:DropDownList ID="ToSurahDropDown" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group range-ayah" style="display:none;">
                    <label class="form-label">إلى آية</label>
                    <asp:TextBox ID="ToAyahTb" runat="server" CssClass="form-control" TextMode="Number" />
                </div>
                <div class="form-group range-jozz" style="display:none;">
                    <label class="form-label">من جزء</label>
                    <asp:DropDownList ID="FromJozzDdl" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group range-jozz" style="display:none;">
                    <label class="form-label">إلى جزء</label>
                    <asp:DropDownList ID="ToJozzDdl" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group">
                    <label class="form-label">تاريخ البداية</label>
                    <asp:TextBox ID="FromDateTb" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="form-group">
                    <label class="form-label">تاريخ النهاية</label>
                    <asp:TextBox ID="ToDateTb" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="form-group">
                    <asp:HiddenField ID="EditIdHidden" runat="server" />
                    <asp:HiddenField ID="ReadyPlanEditIdHidden" runat="server" />
                    <asp:Button ID="SaveBtn" runat="server" CssClass="btn btn-primary" Text="حفظ المستوى"
                        OnClick="SaveBtn_Click" ValidationGroup="pg" CausesValidation="true" />
                    <asp:Button ID="CancelEditBtn" runat="server" CssClass="btn btn-secondary" Text="إلغاء"
                        OnClick="CancelEditBtn_Click" CausesValidation="false" />
                    <asp:Button ID="SaveReadyPlanBtn" runat="server" CssClass="btn btn-primary" Text="حفظ خطة جاهزة"
                        OnClick="SaveReadyPlanBtn_Click" CausesValidation="false" />
                </div>
            </div>
        </div>

        <div class="grid-card">
            <h5 style="margin-bottom:12px;">مستويات الخطة (الخاصة بك والعامة)</h5>
            <div class="table-responsive">
                <asp:GridView ID="gvLevels" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                    OnRowCommand="gvLevels_RowCommand"
                    CssClass="table table-striped table-bordered table-hover align-middle" GridLines="None"
                    HeaderStyle-CssClass="table-light">
                    <Columns>
                        <asp:BoundField DataField="LevelName" HeaderText="اسم المستوى" />
                        <asp:TemplateField HeaderText="القدرة">
                            <ItemTemplate>
                                <%# GetUnitDisplay(Eval("UnitType")) %> (<%# Eval("Quantity") %>)
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="النوع">
                            <ItemTemplate>
                                <%# Eval("CreatedByTeacherId")==null ? "<span class='badge-global'>عام</span>"
                                    : "<span class='badge-private'>خاص</span>" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="إجراءات">
                            <ItemTemplate>
                                <asp:PlaceHolder runat="server" Visible='<%# Eval("CreatedByTeacherId") != null %>'>
                                    <asp:LinkButton runat="server" CssClass="btn btn-primary btn-sm"
                                        CommandName="editLevel" CommandArgument='<%# Eval("Id") %>' Text="تعديل" />
                                    <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm"
                                        CommandName="deleteLevel" CommandArgument='<%# Eval("Id") %>' Text="حذف"
                                        OnClientClick="return confirm('تأكيد الحذف؟');" />
                                </asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="alert alert-secondary text-center m-0">لا توجد مستويات بعد</div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <div class="grid-card" style="margin-top:20px;">
            <h5 style="margin-bottom:12px;">الخطط الجاهزة (الخاصة بك والعامة)</h5>
            <div class="table-responsive">
                <asp:GridView ID="gvReadyPlans" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                    OnRowCommand="gvReadyPlans_RowCommand"
                    CssClass="table table-striped table-bordered table-hover align-middle" GridLines="None"
                    HeaderStyle-CssClass="table-light">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="#" />
                        <asp:BoundField DataField="LevelName" HeaderText="المستوى" />
                        <asp:BoundField DataField="FromSurahName" HeaderText="من سورة" />
                        <asp:BoundField DataField="ToSurahName" HeaderText="إلى سورة" />
                        <asp:BoundField DataField="FromDate" HeaderText="من تاريخ" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="ToDate" HeaderText="إلى تاريخ" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:TemplateField HeaderText="النوع">
                            <ItemTemplate>
                                <%# Eval("CreatedByTeacherId")==null ? "<span class='badge-global'>عام</span>"
                                    : "<span class='badge-private'>خاص</span>" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="إجراءات">
                            <ItemTemplate>
                                <asp:PlaceHolder runat="server" Visible='<%# Eval("CreatedByTeacherId") != null %>'>
                                    <asp:LinkButton runat="server" CssClass="btn btn-primary btn-sm"
                                        CommandName="editReadyPlan" CommandArgument='<%# Eval("Id") %>' Text="تعديل" />
                                    <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm"
                                        CommandName="deleteReadyPlan" CommandArgument='<%# Eval("Id") %>' Text="حذف"
                                        OnClientClick="return confirm('تأكيد الحذف؟');" />
                                </asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="alert alert-secondary text-center m-0">لا توجد خطط جاهزة بعد</div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
        <script>
            function toggleInputs() {
                var unitTypeDdl = document.getElementById('<%= UnitTypeDdl.ClientID %>');
                if (!unitTypeDdl) return;
                var unitType = unitTypeDdl.value;
                var surahGroups = document.querySelectorAll('.range-surah');
                var ayahGroups = document.querySelectorAll('.range-ayah');
                var jozzGroups = document.querySelectorAll('.range-jozz');

                if (unitType === '2') { // Jozz
                    surahGroups.forEach(function (g) { g.style.display = 'none'; });
                    ayahGroups.forEach(function (g) { g.style.display = 'none'; });
                    jozzGroups.forEach(function (g) { g.style.display = 'block'; });
                } else { // Page or Quarter Page
                    surahGroups.forEach(function (g) { g.style.display = 'block'; });
                    ayahGroups.forEach(function (g) { g.style.display = 'none'; });
                    jozzGroups.forEach(function (g) { g.style.display = 'none'; });
                }
            }

            window.addEventListener('load', toggleInputs);
        </script>
    </asp:Content>