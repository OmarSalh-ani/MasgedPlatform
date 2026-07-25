<%@ Page Title="خطة الطالب" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="StudentPlan2.aspx.cs" Inherits="masabihq8Teacher.StudentPlan2" EnableEventValidation="false" %>
    <%@ Register Src="/AssignPlanModalControl.ascx" TagPrefix="uc1" TagName="AssignPlanModalControl" %>
        <%@ Register Src="/PlanTabControl.ascx" TagPrefix="uc1" TagName="PlanTabControl" %>

            <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
                <style>
                    @import url('https://fonts.googleapis.com/css2?family=Cairo:wght@200;300;400;500;600;700;800;900&display=swap');

                    :root {
                        --primary-color: #7C8738;
                        --secondary-color: #CBAC2D;
                        --text-color: #333;
                        --border-radius: 12px;
                        --card-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                    }

                    body {
                        font-family: 'Cairo', sans-serif;
                        direction: rtl;
                    }

                    .plan-container {
                        padding: 20px 10px;
                    }

                    .page-header {
                        background: linear-gradient(135deg, var(--primary-color), #1a5f8a);
                        color: white;
                        padding: 25px;
                        border-radius: var(--border-radius);
                        margin-bottom: 25px;
                        box-shadow: var(--card-shadow);
                    }

                    .page-title {
                        font-size: 1.8rem;
                        font-weight: 700;
                        margin: 0;
                        text-align: center;
                    }

                    /* Allow dropdown panels to extend outside the card (master has .content-card overflow:hidden) */
                    .content-card {
                        overflow: visible !important;
                    }

                    .form-card {
                        background: white;
                        padding: 25px;
                        border-radius: var(--border-radius);
                        box-shadow: var(--card-shadow);
                        margin-bottom: 25px;
                        overflow: visible;
                    }

                    .form-label {
                        font-weight: 600;
                        color: var(--text-color);
                        margin-bottom: 6px;
                        display: block;
                    }

                    .form-control-plan {
                        padding: 8px 12px;
                        border: 2px solid #e9ecef;
                        border-radius: 8px;
                        font-family: Cairo, sans-serif;
                        width: 100%;
                    }

                    .plan-table {
                        width: 100%;
                        border-collapse: collapse;
                        margin-top: 15px;
                        overflow: visible;
                    }

                    .plan-table th,
                    .plan-table td {
                        padding: 10px;
                        border: 1px solid #dee2e6;
                        text-align: right;
                        overflow: visible;
                    }

                    .plan-table th {
                        background: #f8f9fa;
                        font-weight: 600;
                    }

                    .plan-row td,
                    .plan-edit-row td {
                        vertical-align: middle;
                    }

                    .btn-add-row,
                    .btn-save-plan {
                        padding: 10px 20px;
                        border-radius: 25px;
                        font-weight: 600;
                        border: none;
                        cursor: pointer;
                        font-family: Cairo, sans-serif;
                        margin-left: 8px;
                    }

                    .btn-add-row {
                        background: linear-gradient(135deg, #17a2b8, #138496);
                        color: white;
                    }

                    .btn-save-plan {
                        background: linear-gradient(135deg, #28a745, #20c997);
                        color: white;
                    }

                    .btn-add-row:hover,
                    .btn-save-plan:hover {
                        opacity: 0.9;
                        transform: translateY(-1px);
                    }

                    .btn-add-row:disabled,
                    .btn-save-plan:disabled {
                        opacity: 0.5;
                        cursor: not-allowed;
                        transform: none;
                        pointer-events: none;
                    }

                    .btn-add-row:disabled:hover,
                    .btn-save-plan:disabled:hover {
                        opacity: 0.5;
                        transform: none;
                    }

                    .plans-list-card {
                        background: white;
                        padding: 25px;
                        border-radius: var(--border-radius);
                        box-shadow: var(--card-shadow);
                        margin-bottom: 25px;
                    }

                    .plans-table {
                        width: 100%;
                        border-collapse: collapse;
                    }

                    .plans-table th,
                    .plans-table td {
                        padding: 10px;
                        border: 1px solid #dee2e6;
                        text-align: right;
                    }

                    .plans-table th {
                        background: #f8f9fa;
                        font-weight: 600;
                    }

                    .btn-edit,
                    .btn-delete,
                    .btn-quran {
                        padding: 6px 12px;
                        border-radius: 8px;
                        border: none;
                        cursor: pointer;
                        font-size: 0.85rem;
                        margin-left: 4px;
                        text-decoration: none;
                        display: inline-block;
                    }

                    .btn-edit {
                        background: #17a2b8;
                        color: white;
                    }

                    .btn-delete {
                        background: #dc3545;
                        color: white;
                    }

                    .cal-type-tathbit {
                        background: #e67e22;
                        color: white;
                    }

                    .btn-quran {
                        background: #28a745;
                        color: white;
                    }

                    .student-name-list {
                        max-height: 200px;
                        overflow-y: auto;
                        border: 2px solid #e9ecef;
                        border-radius: 8px;
                        padding: 8px;
                    }

                    .no-plans {
                        text-align: center;
                        padding: 30px;
                        color: #666;
                    }

                    .back-link {
                        display: inline-block;
                        margin-bottom: 15px;
                        color: var(--primary-color);
                    }

                    .edit-mode-toggle-wrap {
                        display: flex;
                        align-items: center;
                        gap: 8px;
                    }

                    .edit-mode-toggle-wrap input[type="checkbox"] {
                        width: 18px;
                        height: 18px;
                        cursor: pointer;
                    }

                    .edit-mode-label {
                        cursor: pointer;
                        font-size: 0.95rem;
                    }

                    /* Plan item checkboxes for multi-selection */
                    .plan-checkbox-cell {
                        width: 40px;
                        text-align: center;
                        vertical-align: middle;
                    }

                    .plan-checkbox-cell input[type="checkbox"] {
                        width: 18px;
                        height: 18px;
                        cursor: pointer;
                    }

                    .plan-select-all-btn,
                    .plan-deselect-all-btn {
                        padding: 6px 12px;
                        border-radius: 6px;
                        font-size: 0.85rem;
                        font-weight: 600;
                        border: 1px solid #ddd;
                        background: #f8f9fa;
                        cursor: pointer;
                        margin-left: 5px;
                        transition: all 0.2s;
                    }

                    .plan-select-all-btn:hover,
                    .plan-deselect-all-btn:hover {
                        background: #e9ecef;
                        border-color: #ccc;
                    }

                    .next-memorize-btn {
                        padding: 10px 20px;
                        border-radius: 25px;
                        font-weight: 600;
                        border: none;
                        background: linear-gradient(135deg, #28a745, #20c997);
                        color: white;
                        cursor: pointer;
                        margin-top: 15px;
                        transition: all 0.3s;
                    }

                    .next-memorize-btn:hover {
                        background: linear-gradient(135deg, #218838, #1ba87e);
                        transform: translateY(-2px);
                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
                    }

                    .next-revise-btn {
                        padding: 10px 20px;
                        border-radius: 25px;
                        font-weight: 600;
                        border: none;
                        background: linear-gradient(135deg, #007bff, #0056b3);
                        color: white;
                        cursor: pointer;
                        margin-top: 15px;
                        margin-left: 10px;
                        transition: all 0.3s;
                    }

                    .next-revise-btn:hover {
                        background: linear-gradient(135deg, #0056b3, #004085);
                        transform: translateY(-2px);
                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
                    }

                    /* Students dropdown with checkboxes + search */
                    .students-dropdown {
                        position: relative;
                        max-width: 400px;
                    }

                    .students-dropdown-trigger {
                        display: flex;
                        align-items: center;
                        justify-content: space-between;
                        width: 100%;
                        padding: 10px 14px;
                        border: 2px solid #e9ecef;
                        border-radius: 8px;
                        background: white;
                        font-family: Cairo, sans-serif;
                        font-size: 0.95rem;
                        text-align: right;
                        cursor: pointer;
                        transition: border-color 0.2s;
                    }

                    .students-dropdown-trigger:hover {
                        border-color: var(--primary-color);
                    }

                    .students-dropdown-trigger i.fa-chevron-down {
                        margin-left: 8px;
                        transition: transform 0.2s;
                    }

                    .students-dropdown.open .students-dropdown-trigger i.fa-chevron-down {
                        transform: rotate(180deg);
                    }

                    .students-dropdown-panel {
                        display: none;
                        position: absolute;
                        top: 100%;
                        right: 0;
                        left: 0;
                        margin-top: 4px;
                        background: white;
                        border: 2px solid #e9ecef;
                        border-radius: 8px;
                        box-shadow: 0 8px 20px rgba(0, 0, 0, 0.12);
                        z-index: 100;
                        max-height: 280px;
                        overflow: hidden;
                        flex-direction: column;
                    }

                    .students-dropdown.open .students-dropdown-panel {
                        display: flex;
                    }

                    .students-dropdown-search {
                        padding: 10px;
                        border-bottom: 1px solid #e9ecef;
                        flex-shrink: 0;
                    }

                    .students-dropdown-search input {
                        width: 100%;
                        padding: 8px 12px 8px 32px;
                        border: 1px solid #dee2e6;
                        border-radius: 6px;
                        font-family: Cairo, sans-serif;
                        font-size: 0.9rem;
                    }

                    .students-dropdown-list {
                        overflow-y: auto;
                        padding: 8px;
                        max-height: 220px;
                    }

                    .students-dropdown-option {
                        display: flex;
                        align-items: center;
                        padding: 8px 10px;
                        border-radius: 6px;
                        cursor: pointer;
                    }

                    .students-dropdown-option:hover {
                        background: #f8f9fa;
                    }

                    .students-dropdown-option input[type=checkbox] {
                        position: absolute;
                        width: 0;
                        height: 0;
                        opacity: 0;
                        margin: 0;
                        pointer-events: none;
                    }

                    .students-dropdown-option-name {
                        flex: 1;
                        cursor: pointer;
                    }

                    .students-dropdown-option.hide-by-search {
                        display: none;
                    }

                    .students-dropdown-option.hide-by-circle {
                        display: none;
                    }

                    .students-dropdown-actions {
                        padding: 8px 10px;
                        border-bottom: 1px solid #e9ecef;
                        flex-shrink: 0;
                        display: flex;
                        gap: 8px;
                        flex-wrap: wrap;
                    }

                    .students-select-all-btn,
                    .students-deselect-all-btn {
                        padding: 6px 12px;
                        border-radius: 6px;
                        border: 1px solid var(--primary-color);
                        background: white;
                        color: var(--primary-color);
                        cursor: pointer;
                        font-family: Cairo, sans-serif;
                        font-size: 0.85rem;
                    }

                    .students-select-all-btn:hover,
                    .students-deselect-all-btn:hover {
                        background: #f0f7fc;
                    }

                    .circle-dropdown .students-dropdown-list {
                        max-height: 220px;
                    }

                    .circle-dropdown-option {
                        padding: 8px 10px;
                        border-radius: 6px;
                        cursor: pointer;
                    }

                    .circle-dropdown-option:hover {
                        background: #f8f9fa;
                    }

                    .circle-dropdown-option.hide-by-search {
                        display: none;
                    }

                    .circle-students-row {
                        display: flex;
                        flex-wrap: wrap;
                        gap: 20px;
                        align-items: flex-end;
                        margin-bottom: 20px;
                    }

                    .circle-students-row .form-label {
                        display: block;
                        margin-bottom: 6px;
                    }

                    .circle-students-one-row {
                        display: flex;
                        flex-wrap: wrap;
                        gap: 20px;
                        align-items: flex-end;
                        margin-bottom: 20px;
                    }

                    .circle-students-one-row .circle-cell {
                        flex: 0 0 auto;
                    }

                    .circle-students-one-row .students-cell {
                        flex: 1;
                        min-width: 200px;
                    }

                    .circle-students-one-row .form-label {
                        margin-bottom: 6px;
                    }

                    .selected-students-row {
                        width: 100%;
                        margin-top: 12px;
                        margin-bottom: 20px;
                    }

                    .selected-students-row .form-label {
                        display: block;
                        margin-bottom: 6px;
                    }

                    .selected-students-summary {
                        margin-top: 10px;
                    }

                    .selected-students-summary .form-label {
                        margin-bottom: 6px;
                        display: block;
                    }

                    .selected-students-list {
                        display: flex;
                        flex-wrap: wrap;
                        gap: 8px;
                        min-height: 24px;
                        padding: 10px;
                        background: #f8f9fa;
                        border-radius: 8px;
                        border: 1px solid #e9ecef;
                    }

                    .selected-student-tag {
                        display: inline-flex;
                        align-items: center;
                        gap: 6px;
                        padding: 6px 10px;
                        background: var(--primary-color);
                        color: white;
                        border-radius: 20px;
                        font-size: 0.9rem;
                    }

                    .selected-student-tag .tag-remove {
                        cursor: pointer;
                        opacity: 0.9;
                    }

                    .selected-student-tag .tag-remove:hover {
                        opacity: 1;
                    }

                    .selected-students-empty {
                        color: #6c757d;
                        font-size: 0.9rem;
                    }

                    /* Searchable dropdown (surah, from ayah, to ayah, type) */
                    .searchable-dropdown {
                        position: relative;
                        min-width: 120px;
                    }

                    .searchable-dropdown select {
                        position: absolute;
                        width: 0;
                        height: 0;
                        opacity: 0;
                        pointer-events: none;
                        margin: 0;
                        padding: 0;
                        border: 0;
                    }

                    .searchable-dropdown-trigger {
                        display: flex;
                        align-items: center;
                        justify-content: space-between;
                        width: 100%;
                        padding: 8px 12px;
                        border: 2px solid #e9ecef;
                        border-radius: 8px;
                        background: white;
                        font-family: Cairo, sans-serif;
                        font-size: 0.9rem;
                        text-align: right;
                        cursor: pointer;
                    }

                    .searchable-dropdown-trigger:hover {
                        border-color: var(--primary-color);
                    }

                    .searchable-dropdown-trigger i.fa-chevron-down {
                        margin-left: 8px;
                        transition: transform 0.2s;
                    }

                    .searchable-dropdown.open .searchable-dropdown-trigger i.fa-chevron-down {
                        transform: rotate(180deg);
                    }

                    .searchable-dropdown-panel {
                        display: none;
                        position: absolute;
                        top: 100%;
                        right: 0;
                        left: 0;
                        margin-top: 4px;
                        background: white;
                        border: 2px solid #e9ecef;
                        border-radius: 8px;
                        box-shadow: 0 8px 20px rgba(0, 0, 0, 0.12);
                        z-index: 1050;
                        max-height: 260px;
                        overflow: hidden;
                        flex-direction: column;
                    }

                    .searchable-dropdown.open .searchable-dropdown-panel {
                        display: flex;
                    }

                    .searchable-dropdown-search {
                        padding: 8px;
                        border-bottom: 1px solid #e9ecef;
                        flex-shrink: 0;
                    }

                    .searchable-dropdown-search input {
                        width: 100%;
                        padding: 6px 10px;
                        border: 1px solid #dee2e6;
                        border-radius: 6px;
                        font-family: Cairo, sans-serif;
                        font-size: 0.9rem;
                    }

                    .searchable-dropdown-list {
                        overflow-y: auto;
                        padding: 6px 6px 16px 6px;
                        max-height: 200px;
                    }

                    .searchable-dropdown-option {
                        padding: 6px 10px;
                        border-radius: 6px;
                        cursor: pointer;
                    }

                    .searchable-dropdown-option.hide-by-search {
                        display: none;
                    }

                    /* Keep plan table inside card on mobile */
                    .plan-table-wrap {
                        min-width: 0;
                    }

                    @media (max-width: 768px) {
                        .plan-container {
                            max-width: 100%;
                            overflow-x: hidden;
                            box-sizing: border-box;
                        }

                        .form-card {
                            max-width: 100%;
                            box-sizing: border-box;
                            padding: 15px;
                        }

                        .plan-table-wrap {
                            overflow-x: auto;
                            -webkit-overflow-scrolling: touch;
                            margin: 0 -15px;
                            padding: 0 15px;
                        }

                        .plan-table {
                            min-width: 600px;
                        }
                    }

                    .plan-status-cell {
                        white-space: nowrap;
                    }

                    .plan-status-btn {
                        padding: 4px 10px;
                        margin-left: 4px;
                        border-radius: 6px;
                        border: 1px solid #dee2e6;
                        cursor: pointer;
                        font-size: 0.85rem;
                        background: #f8f9fa;
                    }

                    .plan-status-btn.active {
                        font-weight: 600;
                    }

                    .plan-status-pass.active {
                        background: #28a745;
                        color: white;
                        border-color: #28a745;
                    }

                    .plan-status-fail.active {
                        background: #dc3545;
                        color: white;
                        border-color: #dc3545;
                    }

                    .plan-status-pending.active {
                        background: #6c757d;
                        color: white;
                        border-color: #6c757d;
                    }

                    .plan-status-retake.active {
                        background: #fd7e14;
                        color: white;
                        border-color: #fd7e14;
                    }

                    .plan-status-row[data-status="تم"] {
                        background: rgba(40, 167, 69, 0.08);
                    }

                    .plan-status-row[data-status="لم يتم"] {
                        background: rgba(220, 53, 69, 0.08);
                    }

                    .plan-status-row[data-status="اعادة تسميع"] {
                        background: rgba(253, 126, 20, 0.08);
                    }

                    .assessment-log-table {
                        width: 100%;
                        border-collapse: collapse;
                        margin-top: 10px;
                        font-size: 0.9rem;
                    }

                    .assessment-log-table th,
                    .assessment-log-table td {
                        padding: 8px;
                        border: 1px solid #dee2e6;
                        text-align: right;
                    }

                    .assessment-log-table th {
                        background: #f8f9fa;
                    }

                    .assessment-log-wrap {
                        max-height: 320px;
                        overflow-y: auto;
                        overflow-x: auto;
                        margin-top: 10px;
                    }

                    .btn-quran-link {
                        display: inline-flex;
                        align-items: center;
                        gap: 8px;
                        padding: 10px 20px;
                        border-radius: 25px;
                        font-weight: 600;
                        background: rgba(255, 255, 255, 0.25);
                        color: white;
                        border: 1px solid rgba(255, 255, 255, 0.4);
                        text-decoration: none;
                        transition: all 0.2s;
                    }

                    .btn-quran-link:hover {
                        background: rgba(255, 255, 255, 0.35);
                        color: white;
                        text-decoration: none;
                        transform: translateY(-1px);
                    }

                    .btn-add-plan-link {
                        display: inline-flex;
                        align-items: center;
                        gap: 8px;
                        padding: 10px 20px;
                        border-radius: 25px;
                        font-weight: 600;
                        background: rgba(255, 255, 255, 0.25);
                        color: white;
                        border: 1px solid rgba(255, 255, 255, 0.4);
                        text-decoration: none;
                        transition: all 0.2s;
                    }

                    .btn-add-plan-link:hover {
                        background: rgba(255, 255, 255, 0.35);
                        color: white;
                        text-decoration: none;
                        transform: translateY(-1px);
                    }

                    .btn-delete-plan-link {
                        display: inline-flex;
                        align-items: center;
                        gap: 8px;
                        padding: 10px 20px;
                        border-radius: 25px;
                        font-weight: 600;
                        background: rgba(220, 53, 69, 0.8);
                        color: white;
                        border: 1px solid rgba(255, 255, 255, 0.4);
                        text-decoration: none;
                        transition: all 0.2s;
                    }

                    .btn-delete-plan-link:hover {
                        background: rgba(220, 53, 69, 1);
                        color: white;
                        text-decoration: none;
                        transform: translateY(-1px);
                    }

                    /* Tabs Styling */
                    .plan-tabs {
                        display: flex;
                        gap: 10px;
                        margin-bottom: 20px;
                        border-bottom: 2px solid #e9ecef;
                        padding-bottom: 10px;
                    }

                    .plan-tab-btn {
                        padding: 10px 20px;
                        border: none;
                        background: none;
                        font-family: 'Cairo', sans-serif;
                        font-size: 1rem;
                        font-weight: 600;
                        color: #666;
                        cursor: pointer;
                        border-radius: 8px;
                        transition: all 0.2s;
                    }

                    .plan-tab-btn:hover {
                        background: #f8f9fa;
                        color: var(--primary-color);
                    }

                    .plan-tab-btn.active {
                        background: var(--primary-color);
                        color: white;
                    }

                    .plan-tab-btn.tab-revise-btn.active {
                        background: linear-gradient(135deg, #007bff, #0056b3);
                        color: white;
                    }

                    .plan-tab-btn.tab-revise-btn:hover {
                        color: #007bff;
                    }

                    .plan-tab-btn.tab-tathbit-btn.active {
                        background: linear-gradient(135deg, #e67e22, #d35400);
                        color: white;
                    }

                    .plan-tab-btn.tab-tathbit-btn:hover {
                        color: #e67e22;
                    }

                    .plan-tab-content {
                        display: none;
                    }

                    .plan-tab-content.active {
                        display: block;
                    }

                    /* Calendar Tab */
                    .cal-grid {
                        display: grid;
                        grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
                        gap: 20px;
                        padding: 10px;
                    }

                    .cal-day-card {
                        background: rgba(255, 255, 255, 0.9);
                        backdrop-filter: blur(10px);
                        border-radius: 16px;
                        box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
                        padding: 18px;
                        border: 1px solid rgba(255, 255, 255, 0.3);
                        border-top: 5px solid #e9ecef;
                        transition: transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1), box-shadow 0.3s;
                        position: relative;
                        overflow: hidden;
                    }

                    .cal-day-card:hover {
                        transform: translateY(-8px);
                        box-shadow: 0 12px 30px rgba(0, 0, 0, 0.1);
                    }

                    .cal-day-card.is-circle {
                        border-top-color: var(--primary-color);
                        background: linear-gradient(to bottom, #f9fffb, #ffffff);
                    }

                    .cal-day-card.is-holiday {
                        border-top-color: #ffc107;
                        background: linear-gradient(to bottom, #fffcf0, #ffffff);
                    }

                    .cal-day-header {
                        display: flex;
                        justify-content: space-between;
                        align-items: center;
                        margin-bottom: 12px;
                        padding-bottom: 8px;
                        border-bottom: 1px dashed #eee;
                    }

                    .cal-date {
                        font-weight: 700;
                        font-size: 1.1rem;
                        color: #2c3e50;
                    }

                    .cal-day-name {
                        font-size: 0.85rem;
                        color: #7f8c8d;
                        background: #f1f3f5;
                        padding: 2px 8px;
                        border-radius: 20px;
                    }

                    .cal-day-content {
                        min-height: 50px;
                    }

                    .cal-holiday-text {
                        color: #f39c12;
                        font-weight: 600;
                        display: flex;
                        align-items: center;
                        gap: 8px;
                        font-size: 1rem;
                        justify-content: center;
                        margin-top: 10px;
                    }

                    .cal-surah-item {
                        display: flex;
                        align-items: center;
                        gap: 10px;
                        margin-bottom: 8px;
                        padding: 8px 12px;
                        background: #f8fcf9;
                        border-radius: 10px;
                        border: 1px solid #eefae1;
                        font-weight: 500;
                        font-size: 0.95rem;
                    }

                    .cal-plan-type {
                        font-size: 0.7rem;
                        padding: 2px 6px;
                        border-radius: 4px;
                        color: white;
                        font-weight: 700;
                        text-transform: uppercase;
                    }

                    .cal-type-mem {
                        background: var(--primary-color);
                    }

                    .cal-type-rev {
                        background: var(--secondary-color);
                    }

                    .cal-quran-icon {
                        margin-right: auto;
                        color: var(--primary-color);
                        font-size: 1.2rem;
                        transition: transform 0.2s, color 0.2s;
                    }

                    .cal-quran-icon:hover {
                        transform: scale(1.2);
                        color: var(--secondary-color);
                    }
                </style>
                <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
            </asp:Content>
            <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
                <div class="plan-container">
                    <div class="page-header">
                        <h1 class="page-title">
                            <asp:Literal ID="LiteralHeaderTitle" runat="server" Text="خطة الطالب" />
                        </h1>
                        <div style="margin-top: 10px; display:flex; gap:10px; flex-wrap:wrap;">


                            <asp:LinkButton ID="DeletePlanLinkBtn" runat="server" CssClass="btn-delete-plan-link"
                                Visible="false" OnClick="DeletePlanLinkBtn_Click"
                                OnClientClick="return confirm('هل أنت متأكد من حذف هذه الخطة بالكامل بجميع بنودها؟');">
                                <i class="fas fa-archive"></i> أرشفة الخطة الحالية
                            </asp:LinkButton>
                        </div>
                    </div>

                    <asp:Panel ID="PanelViewMode" runat="server" Visible="false">
                        <asp:Panel ID="PanelPlanView" runat="server">
                            <div
                                style="margin-bottom: 15px; display: flex; flex-wrap: wrap; align-items: center; gap: 10px;display:none">
                                <span class="form-label" style="margin: 0;">الخطة:</span>
                                <asp:DropDownList ID="PlanNameDropDown" runat="server" CssClass="form-control"
                                    style="max-width: 280px;" AutoPostBack="true"
                                    OnSelectedIndexChanged="PlanNameDropDown_SelectedIndexChanged" />

                            </div>
                            
                            <div class="edit-mode-toggle-wrap" style="margin-bottom: 15px;">
                                <asp:CheckBox ID="EditModeCheckBox" runat="server" AutoPostBack="true"
                                    OnCheckedChanged="EditModeCheckBox_CheckedChanged" CssClass="edit-mode-checkbox" />
                                <label for="EditModeCheckBox" class="edit-mode-label">تفعيل التعديل</label>
                            </div>
                            
                            
                            <div id="planInfoSection" class="plan-info-section"
                                style="background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%); padding: 15px 20px; border-radius: var(--border-radius); margin-bottom: 20px; border: 1px solid #dee2e6;">
                                <h5 style="margin: 0 0 10px 0; font-weight: 700; color: var(--primary-color);">
                                    <i class="fas fa-info-circle" style="margin-left:6px;"></i>معلومات الخطة الحالية
                                </h5>
                                <asp:Literal ID="LiteralPlanInfo" runat="server" />
                            </div>
                            <div class="plan-charts-row"
                                style="display: flex; flex-wrap: wrap; gap: 20px; align-items: flex-start; margin-bottom: 20px;">
                                <div class="plan-chart-box" style="min-width: 180px; max-width: 220px;">
                                    <div class="plan-chart-title"
                                        style="font-weight: 600; margin-bottom: 8px; font-size: 0.95rem;">تقييم البنود
                                    </div>
                                    <canvas id="planProgressChart" width="200" height="200"></canvas>
                                </div>
                                <div class="plan-chart-box" style="min-width: 180px; max-width: 220px;">
                                    <div class="plan-chart-title"
                                        style="font-weight: 600; margin-bottom: 8px; font-size: 0.95rem;">الأيام
                                        المتبقية</div>
                                    <canvas id="planDaysChart" width="200" height="200"></canvas>
                                </div>
                                <div class="plan-chart-box" style="min-width: 180px; max-width: 220px;">
                                    <div class="plan-chart-title"
                                        style="font-weight: 600; margin-bottom: 8px; font-size: 0.95rem;">نسبة الإنجاز %
                                    </div>
                                    <canvas id="planPercentChart" width="200" height="200"></canvas>
                                </div>
                            </div>
                            <div id="progressDataJson" style="display:none;">
                                <asp:Literal ID="LiteralProgressData" runat="server"
                                    Text='{"passed":0,"failed":0,"pending":0,"retake":0,"total":0,"daysRemaining":0,"totalPlanDays":1,"daysElapsed":0,"progressPercent":0}'
                                    EnableViewState="false" />
                            </div>
                            <asp:HiddenField ID="HiddenViewStudentId" runat="server" Value="" />
                            <asp:HiddenField ID="HiddenViewPlanId" runat="server" Value="" />
                        </asp:Panel>
                    </asp:Panel>

                    <div class="form-card">
                        <h3 style="margin-bottom: 20px;">
                            <asp:Literal ID="LiteralFormTitle" runat="server" Text="إضافة خطة" />
                        </h3>
                        <asp:Panel ID="PanelStudentsSelect" runat="server" Visible="true">
                            <div class="circle-students-one-row">
                                <div class="students-cell">
                                    <asp:HiddenField ID="SelectedStudentIds" runat="server" Value="" />
                                    <span class="form-label">الطلاب (اختر واحداً أو أكثر)</span>
                                    <div class="students-dropdown" id="studentsDropdown">
                                        <button type="button" class="students-dropdown-trigger"
                                            id="studentsDropdownTrigger" aria-expanded="false">
                                            <span id="studentsDropdownLabel">اختر الطلاب</span>
                                            <i class="fas fa-chevron-down"></i>
                                        </button>
                                        <div class="students-dropdown-panel" id="studentsDropdownPanel">
                                            <div class="students-dropdown-actions">
                                                <button type="button" class="students-select-all-btn"
                                                    id="studentsSelectAllBtn">تحديد الكل</button>
                                                <button type="button" class="students-deselect-all-btn"
                                                    id="studentsDeselectAllBtn">إلغاء تحديد الكل</button>
                                            </div>
                                            <div class="students-dropdown-search">
                                                <input type="text" id="studentsSearchInput" placeholder="بحث بالاسم..."
                                                    autocomplete="off" />
                                            </div>
                                            <div class="students-dropdown-list">
                                                <asp:Repeater ID="StudentsRepeater" runat="server"
                                                    OnItemDataBound="StudentsRepeater_ItemDataBound">
                                                    <ItemTemplate>
                                                        <div class="students-dropdown-option"
                                                            data-id='<%# Eval("Id") %>'
                                                            data-name='<%# System.Web.HttpUtility.HtmlAttributeEncode((Eval("Name") as string) ?? "") %>'
                                                            data-circle-id='<%# Eval("QuranCircleId") != null ? Eval("QuranCircleId").ToString() : "" %>'>
                                                            <asp:HiddenField ID="StudentIdHidden" runat="server"
                                                                Value='<%# Eval("Id") %>' />
                                                            <asp:CheckBox ID="StudentCheckBox" runat="server"
                                                                CssClass="student-cb" />
                                                            <span class="students-dropdown-option-name">
                                                                <%# Eval("Name") %>
                                                            </span>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="selected-students-row">
                                <span class="form-label">الطلاب المحددون:</span>
                                <div id="selectedStudentsList" class="selected-students-list">
                                    <span class="selected-students-empty" id="selectedStudentsEmpty">لم يتم اختيار
                                        طلاب</span>
                                </div>
                            </div>
                        </asp:Panel>
                      

                        <div class="plan-tabs">
                            <asp:HiddenField ID="HiddenActiveTab" runat="server" Value="tab-single" />
                            <button type="button" class="plan-tab-btn active" data-tab="tab-single"
                                onclick="switchPlanTab(event, 'tab-single')">بنود الحفظ</button>
                            <button type="button" class="plan-tab-btn tab-revise-btn" data-tab="tab-revise"
                                onclick="switchPlanTab(event, 'tab-revise')"><i class="fas fa-sync-alt"
                                    style="margin-left:6px;"></i>مراجعة</button>
                            <button type="button" class="plan-tab-btn tab-tathbit-btn" data-tab="tab-tathbit"
                                onclick="switchPlanTab(event, 'tab-tathbit')"><i class="fas fa-check-double"
                                    style="margin-left:6px;"></i>تثبيت</button>
                            <button type="button" class="plan-tab-btn" data-tab="tab-by-level" style="display:none"
                                onclick="switchPlanTab(event, 'tab-by-level')">اضافة سور حسب مستوى الحفظ</button>
                            <button type="button" class="plan-tab-btn" data-tab="tab-calendar"
                                onclick="switchPlanTab(event, 'tab-calendar')">تقويم</button>
                            <button type="button" class="plan-tab-btn" data-tab="tab-assessment-log"
                                onclick="switchPlanTab(event, 'tab-assessment-log')">سجل التقييم</button>
                        </div>

                        <uc1:AssignPlanModalControl runat="server" id="AssignPlanModalControl" />

                        <div id="tab-single" class="plan-tab-content active">
                            <div style="margin-top: 20px;">
                                <uc1:PlanTabControl ID="TabSingleCtrl" runat="server" PlanType="حفظ"
                                    HidePlanTypeColumn="true"
                                    EmptyMessage="لا توجد بنود في خطة الحفظ. أضف بنوداً أدناه."
                                    OnAddRowClick="AddRowButton_Click" OnSavePlanClick="SavePlanButton_Click"
                                    OnItemCommand="PlansRepeater_ItemCommand"
                                    OnItemDataBound="PlansRepeaterEdit_ItemDataBound" />
                            </div>
                        </div>

                        <!-- ==================== TAB: مراجعة ==================== -->
                        <div id="tab-revise" class="plan-tab-content">
                            <div style="margin-top: 20px;">
                                <div
                                    style="display:flex; justify-content:space-between; align-items:center; margin-bottom: 15px;">
                                    <h4 style="color: #007bff; margin:0;"><i class="fas fa-sync-alt"
                                            style="margin-left:6px;"></i>بنود المراجعة</h4>
                                    <button type="button" class="btn btn-primary"
                                        onclick="window.showAssignRevisePlanModal()">
                                        <i class="fas fa-tasks" style="margin-left:5px;"></i>إنشاء خطة للمراجعة
                                    </button>
                                </div>

                                <script>
                                    (function () {
                                        var hidSid = document.getElementById('<%= HiddenViewStudentId.ClientID %>');
                                        var hidPid = document.getElementById('<%= HiddenViewPlanId.ClientID %>');
                                        if (hidSid && hidPid && hidSid.value && hidPid.value) {
                                            window.assignReviseContext = { studentId: parseInt(hidSid.value, 10), planId: parseInt(hidPid.value, 10) };
                                        } else {
                                            window.assignReviseContext = null;
                                        }
                                    })();
                                    function showAssignRevisePlanModal() {
                                        var modal = document.getElementById('assignPlanModal');
                                        if (modal) {
                                            modal.style.display = 'block';
                                            // 1) Set plan type to مراجعة
                                            var typeDropdown = modal.querySelector('.ddlAssignPlanTypeClass');
                                            if (typeDropdown) typeDropdown.value = 'مراجعة';
                                            // 2) Pre-select current student
                                            if (window.assignReviseContext && window.assignReviseContext.studentId) {
                                                modal.querySelectorAll('.assign-student-cb').forEach(function (cb) {
                                                    cb.checked = (parseInt(cb.value, 10) === window.assignReviseContext.studentId);
                                                });
                                            }
                                        }
                                    }
                                </script>

                                <div style="display:none;">
                                    <asp:TextBox ID="ReviseDate" runat="server" TextMode="Date" />
                                </div>

                                <uc1:PlanTabControl ID="TabReviseCtrl" runat="server" PlanType="مراجعة"
                                    HidePlanTypeColumn="true" EmptyMessage="لا توجد بنود مراجعة في هذه الخطة."
                                    AddButtonText="+ إضافة سطر مراجعة" SaveButtonText="حفظ المراجعة"
                                    OnAddRowClick="AddReviseRowButton_Click" OnSavePlanClick="SaveReviseButton_Click"
                                    OnItemCommand="PlansRepeater_ItemCommand"
                                    OnItemDataBound="PlansRepeaterEdit_ItemDataBound" />
                            </div>
                        </div>


                        <!-- ==================== TAB: تثبيت ==================== -->
                        <div id="tab-tathbit" class="plan-tab-content">
                            <div style="margin-top: 20px;">
                                <uc1:PlanTabControl ID="TabTathbitCtrl" runat="server" PlanType="تثبيت"
                                    HidePlanTypeColumn="true" EmptyMessage="لا توجد بنود تم تقييمها بـ 'تم' حتى الآن."
                                    ShowActions="false" OnItemCommand="PlansRepeater_ItemCommand"
                                    OnItemDataBound="PlansRepeaterEdit_ItemDataBound" />
                            </div>
                        </div>

                        <div id="tab-by-level" class="plan-tab-content">
                            <asp:Panel ID="PanelByLevelSurahs" runat="server" CssClass="form-card"
                                style="border: 2px solid #198754; background: #f1fff6; margin-bottom: 25px;">
                                <h4 style="margin-bottom: 15px; color: #198754; font-weight: 700;"><i
                                        class="fas fa-list-ul"></i> اضافة سور حسب مستوى الحفظ</h4>
                                <div style="display: flex; flex-wrap: wrap; gap: 20px; align-items: flex-end;">
                                    <div style="flex: 1; min-width: 200px;">
                                        <span class="form-label">من سورة</span>
                                        <div class="searchable-dropdown">
                                            <asp:DropDownList ID="ByLevelSurahFromDropDown" runat="server"
                                                CssClass="bylevel-surah-from" DataTextField="NameAr"
                                                DataValueField="Id" />
                                            <button type="button" class="searchable-dropdown-trigger"
                                                aria-expanded="false">
                                                <span class="searchable-dropdown-label">-- اختر سورة البداية --</span>
                                                <i class="fas fa-chevron-down"></i>
                                            </button>
                                            <div class="searchable-dropdown-panel">
                                                <div class="searchable-dropdown-search">
                                                    <input type="text" class="searchable-dropdown-search-input"
                                                        placeholder="بحث..." autocomplete="off" />
                                                </div>
                                                <div class="searchable-dropdown-list"></div>
                                            </div>
                                        </div>
                                    </div>
                                    <div style="flex: 1; min-width: 200px;">
                                        <span class="form-label">إلى سورة</span>
                                        <div class="searchable-dropdown">
                                            <asp:DropDownList ID="ByLevelSurahToDropDown" runat="server"
                                                CssClass="bylevel-surah-to" DataTextField="NameAr"
                                                DataValueField="Id" />
                                            <button type="button" class="searchable-dropdown-trigger"
                                                aria-expanded="false">
                                                <span class="searchable-dropdown-label">-- اختر سورة النهاية --</span>
                                                <i class="fas fa-chevron-down"></i>
                                            </button>
                                            <div class="searchable-dropdown-panel">
                                                <div class="searchable-dropdown-search">
                                                    <input type="text" class="searchable-dropdown-search-input"
                                                        placeholder="بحث..." autocomplete="off" />
                                                </div>
                                                <div class="searchable-dropdown-list"></div>
                                            </div>
                                        </div>
                                    </div>
                                    <div>
                                        <span class="form-label">نوع الخطة</span>
                                        <div class="searchable-dropdown" style="min-width: 150px;">
                                            <asp:DropDownList ID="ByLevelTypeDropDown" runat="server">
                                                <asp:ListItem Value="حفظ" Text="حفظ" />
                                                <asp:ListItem Value="مراجعة" Text="مراجعة" />
                                            </asp:DropDownList>
                                            <button type="button" class="searchable-dropdown-trigger"
                                                aria-expanded="false">
                                                <span class="searchable-dropdown-label">حفظ</span>
                                                <i class="fas fa-chevron-down"></i>
                                            </button>
                                            <div class="searchable-dropdown-panel">
                                                <div class="searchable-dropdown-search">
                                                    <input type="text" class="searchable-dropdown-search-input"
                                                        placeholder="بحث..." autocomplete="off" />
                                                </div>
                                                <div class="searchable-dropdown-list"></div>
                                            </div>
                                        </div>
                                    </div>
                                   
                                </div>
                                <p style="font-size: 0.85rem; color: #666; margin-top: 10px;"><i
                                        class="fas fa-info-circle"></i>
                                    سيتم إضافة السور من النطاق المحدد كاملة، مع حفظ مستوى الحفظ المختار ضمن بنود الخطة.
                                </p>
                            </asp:Panel>
                        </div>

                        <div id="tab-calendar" class="plan-tab-content">
                            <div class="cal-grid">
                                <asp:Repeater ID="CalendarRepeater" runat="server">
                                    <ItemTemplate>
                                        <div
                                            class='<%# "cal-day-card " + ((bool)Eval("IsCircleDay") ? "is-circle" : "is-holiday") %>'>
                                            <div class="cal-day-header">
                                                <span class="cal-date">
                                                    <%# ((DateTime)Eval("Date")).ToString("dd/MM") %>
                                                </span>
                                                <span class="cal-day-name">
                                                    <%# ((DateTime)Eval("Date")).ToString("dddd", new
                                                        System.Globalization.CultureInfo("ar-KW")) %>
                                                </span>
                                            </div>
                                            <div class="cal-day-content">
                                                <%# Eval("ItemsHtml") %>
                                                    <%# !(bool)Eval("IsCircleDay")
                                                        ? "<div class='cal-holiday-text'><i class='fas fa-umbrella-beach'></i> اجازة</div>"
                                                        : "" %>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>



                        <div id="tab-assessment-log" class="plan-tab-content">
                            <asp:Panel ID="PanelAssessmentLog" runat="server" Visible="false" style="margin-top: 25px;">
                                <h4 style="margin-bottom: 10px;">سجل التقييم</h4>
                                <p style="font-size: 0.9rem; color: #6c757d; margin-bottom: 10px;">كل ضغطة على تم أو لم
                                    يتم
                                    أو
                                    قيد الانتظار أو اعادة تسميع تُسجّل هنا مع الوقت والمعلم.</p>
                                <div class="assessment-log-wrap">
                                    <table class="assessment-log-table">
                                        <thead>
                                            <tr>
                                                <th>السورة</th>
                                                <th>الحالة</th>
                                                <th>المعلم</th>
                                                <th>التاريخ</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater ID="RepeaterAssessmentLog" runat="server">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td>
                                                            <%# Eval("RowLabel") %>
                                                        </td>
                                                        <td>
                                                            <%# Eval("StatusDisplay") %>
                                                        </td>
                                                        <td>
                                                            <%# Eval("TeacherName") %>
                                                        </td>
                                                        <td>
                                                            <%# Eval("LoggedAtFormatted") %>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>
                                <asp:Panel ID="PanelAssessmentLogEmpty" runat="server" Visible="false"
                                    style="margin-top: 10px; padding: 15px; background: #f8f9fa; border-radius: 8px; color: #6c757d; text-align: center;">
                                    لا توجد تقييمات مسجّلة بعد.
                                </asp:Panel>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
                <script type="text/javascript">
                    function switchPlanTab(evt, tabId) {
                        var i, tabcontent, tablinks;
                        tabcontent = document.getElementsByClassName("plan-tab-content");
                        for (i = 0; i < tabcontent.length; i++) {
                            tabcontent[i].classList.remove("active");
                        }
                        tablinks = document.getElementsByClassName("plan-tab-btn");
                        for (i = 0; i < tablinks.length; i++) {
                            tablinks[i].classList.remove("active");
                        }
                        document.getElementById(tabId).classList.add("active");
                        var tabBtn = evt && evt.currentTarget ? evt.currentTarget : document.querySelector('.plan-tab-btn[data-tab="' + tabId + '"]');
                        if (tabBtn) tabBtn.classList.add("active");
                        var hiddenString = '<%= HiddenActiveTab.ClientID %>';
                        var hidden = document.getElementById(hiddenString);
                        if (hidden) hidden.value = tabId;
                    }
                    function switchToTab(tabId) { switchPlanTab(null, tabId); }

                    function restoreActiveTab() {
                        // First check URL query param ?tab=xxx
                        var urlParams = new URLSearchParams(window.location.search);
                        var tabFromUrl = urlParams.get('tab');
                        var hiddenString = '<%= HiddenActiveTab.ClientID %>';
                        var hidden = document.getElementById(hiddenString);
                        var tabId = tabFromUrl || (hidden && hidden.value ? hidden.value : '');
                        if (!tabId) return;
                        var tabBtn = document.querySelector('.plan-tab-btn[data-tab="' + tabId + '"]');
                        if (tabBtn) {
                            var tabcontent = document.getElementsByClassName("plan-tab-content");
                            for (var i = 0; i < tabcontent.length; i++) {
                                tabcontent[i].classList.remove("active");
                            }
                            var tablinks = document.getElementsByClassName("plan-tab-btn");
                            for (var i = 0; i < tablinks.length; i++) {
                                tablinks[i].classList.remove("active");
                            }
                            var targetContent = document.getElementById(tabId);
                            if (targetContent) targetContent.classList.add("active");
                            tabBtn.classList.add("active");
                            if (hidden) hidden.value = tabId;
                        }
                    }

                    document.addEventListener('DOMContentLoaded', function () {
                        restoreActiveTab();
                        if (typeof window.applyProgressiveReveal === 'function') window.applyProgressiveReveal();
                    });

                    if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                            restoreActiveTab();
                            if (typeof window.applyProgressiveReveal === 'function') window.applyProgressiveReveal();
                        });
                    }

                    

                    (function () {
                        try { localStorage.removeItem('StudentPlanRows'); } catch (e) { }
                        var PLAN_ROWS_STORAGE_KEY = 'StudentPlanRows';

                        function getPlanRowsData() {
                            var rows = document.querySelectorAll('.plan-row');
                            var data = [];
                            for (var i = 0; i < rows.length; i++) {
                                var row = rows[i];
                                var surahSelect = row.querySelector('.plan-surah');
                                var fromDdl = row.querySelector('.plan-from-ayah');
                                var toDdl = row.querySelector('.plan-to-ayah');
                                var typeDdl = row.querySelector('.plan-type');
                                var surahId = surahSelect && surahSelect.value ? parseInt(surahSelect.value, 10) : 0;
                                var fromAyah = fromDdl && fromDdl.value ? parseInt(fromDdl.value, 10) : 0;
                                var toAyah = toDdl && toDdl.value ? parseInt(toDdl.value, 10) : 0;
                                var planType = (typeDdl && typeDdl.value) ? typeDdl.value : 'حفظ';
                                data.push({ surahId: surahId, fromAyah: fromAyah, toAyah: toAyah, planType: planType });
                            }
                            return data;
                        }

                        function savePlanRowsToStorage() {
                            try {
                                var data = getPlanRowsData();
                                localStorage.setItem(PLAN_ROWS_STORAGE_KEY, JSON.stringify(data));
                            } catch (e) { }
                        }

                        function updateRowLabels(row) {
                            var fromWrap = row.querySelector('.plan-from-ayah') && row.querySelector('.plan-from-ayah').closest('.searchable-dropdown');
                            var toWrap = row.querySelector('.plan-to-ayah') && row.querySelector('.plan-to-ayah').closest('.searchable-dropdown');
                            var typeWrap = row.querySelector('.plan-type') && row.querySelector('.plan-type').closest('.searchable-dropdown');
                            var fromDdl = row.querySelector('.plan-from-ayah');
                            var toDdl = row.querySelector('.plan-to-ayah');
                            var typeDdl = row.querySelector('.plan-type');
                            if (fromWrap && fromDdl) { var lbl = fromWrap.querySelector('.searchable-dropdown-label'); if (lbl) lbl.textContent = fromDdl.options[fromDdl.selectedIndex] ? fromDdl.options[fromDdl.selectedIndex].text : '--'; }
                            if (toWrap && toDdl) { var lbl = toWrap.querySelector('.searchable-dropdown-label'); if (lbl) lbl.textContent = toDdl.options[toDdl.selectedIndex] ? toDdl.options[toDdl.selectedIndex].text : '--'; }
                            if (typeWrap && typeDdl) { var lbl = typeWrap.querySelector('.searchable-dropdown-label'); if (lbl) lbl.textContent = typeDdl.options[typeDdl.selectedIndex] ? typeDdl.options[typeDdl.selectedIndex].text : 'حفظ'; }
                        }

                        function restorePlanRowsFromStorage() {
                            try {
                                var raw = localStorage.getItem(PLAN_ROWS_STORAGE_KEY);
                                if (!raw) return;
                                var saved = JSON.parse(raw);
                                if (!Array.isArray(saved) || saved.length === 0) return;
                                var rows = document.querySelectorAll('.plan-row');
                                var promises = [];
                                for (var i = 0; i < rows.length; i++) {
                                    (function (idx) {
                                        var row = rows[idx];
                                        var s = saved[idx];
                                        if (!s || !s.surahId) return;
                                        var surahSelect = row.querySelector('.plan-surah');
                                        var fromDdl = row.querySelector('.plan-from-ayah');
                                        var toDdl = row.querySelector('.plan-to-ayah');
                                        var typeDdl = row.querySelector('.plan-type');
                                        if (!surahSelect || !fromDdl || !toDdl) return;
                                        surahSelect.value = String(s.surahId);
                                        var surahWrap = surahSelect.closest('.searchable-dropdown');
                                        if (surahWrap) { var sl = surahWrap.querySelector('.searchable-dropdown-label'); if (sl) sl.textContent = surahSelect.options[surahSelect.selectedIndex] ? surahSelect.options[surahSelect.selectedIndex].text : '--'; }
                                        var p = fetch('StudentPlan2.aspx/GetAyahsBySurah', {
                                            method: 'POST',
                                            headers: { 'Content-Type': 'application/json' },
                                            body: JSON.stringify({ surahId: s.surahId })
                                        }).then(function (r) { return r.json(); }).then(function (res) {
                                            var list = res.d || [];
                                            fromDdl.innerHTML = '<option value="">--</option>';
                                            toDdl.innerHTML = '<option value="">--</option>';
                                            for (var j = 0; j < list.length; j++) {
                                                var v = String(list[j].AyahNumber);
                                                fromDdl.appendChild(new Option(v, v));
                                                toDdl.appendChild(new Option(v, v));
                                            }
                                            fromDdl.value = String(s.fromAyah || '');
                                            toDdl.value = String(s.toAyah || '');
                                            if (typeDdl) typeDdl.value = s.planType || 'حفظ';
                                            updateRowLabels(row);
                                        });
                                        promises.push(p);
                                    })(i);
                                }
                                Promise.all(promises).then(function () {
                                    if (typeof window.reinitPlanSearchableDropdowns === 'function') window.reinitPlanSearchableDropdowns();
                                }).catch(function () {
                                    if (typeof window.reinitPlanSearchableDropdowns === 'function') window.reinitPlanSearchableDropdowns();
                                });
                            } catch (e) { }
                        }

                        window.savePlanRowsToStorage = savePlanRowsToStorage;
                        window.restorePlanRowsFromStorage = restorePlanRowsFromStorage;

                        function loadAyahsForRow(surahSelect) {
                            var surahId = parseInt(surahSelect.value, 10);
                            var tab = surahSelect.closest('.plan-tab-content');
                            var row = surahSelect.closest('tr');
                            if (tab && row && !tab.contains(row)) row = null;
                            if (!row) row = surahSelect.closest('tr');
                            var fromDdl = null, toDdl = null;
                            var surahTd = surahSelect.closest('td');
                            if (surahTd && (!tab || tab.contains(surahTd))) {
                                var fromTd = surahTd.nextElementSibling;
                                var toTd = fromTd ? fromTd.nextElementSibling : null;
                                if (fromTd && (!tab || tab.contains(fromTd))) fromDdl = fromTd.querySelector('select.plan-from-ayah');
                                if (toTd && (!tab || tab.contains(toTd))) toDdl = toTd.querySelector('select.plan-to-ayah');
                            }
                            if (!fromDdl || !toDdl) {
                                if (row) {
                                    fromDdl = row.querySelector('select.plan-from-ayah');
                                    toDdl = row.querySelector('select.plan-to-ayah');
                                }
                            }
                            if (!surahId) {
                                if (fromDdl) { fromDdl.innerHTML = '<option value="">--</option>'; fromDdl.disabled = true; }
                                if (toDdl) { toDdl.innerHTML = '<option value="">--</option>'; toDdl.disabled = true; }
                                var fromWrap = fromDdl && fromDdl.closest('.searchable-dropdown');
                                var toWrap = toDdl && toDdl.closest('.searchable-dropdown');
                                if (fromWrap) { var lbl = fromWrap.querySelector('.searchable-dropdown-label'); if (lbl) lbl.textContent = '--'; }
                                if (toWrap) { var lbl = toWrap.querySelector('.searchable-dropdown-label'); if (lbl) lbl.textContent = '--'; }
                                return;
                            }
                            if (!fromDdl || !toDdl) return;
                            fromDdl.innerHTML = '<option value="">جاري التحميل...</option>';
                            toDdl.innerHTML = '<option value="">جاري التحميل...</option>';
                            fromDdl.disabled = toDdl.disabled = true;
                            fetch('StudentPlan2.aspx/GetAyahsBySurah', {
                                method: 'POST',
                                headers: { 'Content-Type': 'application/json' },
                                body: JSON.stringify({ surahId: surahId })
                            })
                                .then(function (r) { return r.json(); })
                                .then(function (data) {
                                    var list = data.d || [];
                                    fromDdl.innerHTML = '<option value="">--</option>';
                                    toDdl.innerHTML = '<option value="">--</option>';
                                    for (var i = 0; i < list.length; i++) {
                                        var v = list[i].AyahNumber.toString();
                                        var o1 = document.createElement('option'); o1.value = v; o1.textContent = v; fromDdl.appendChild(o1);
                                        var o2 = document.createElement('option'); o2.value = v; o2.textContent = v; toDdl.appendChild(o2);
                                    }
                                    fromDdl.disabled = toDdl.disabled = false;
                                    var fromWrap = fromDdl.closest('.searchable-dropdown');
                                    var toWrap = toDdl.closest('.searchable-dropdown');
                                    if (fromWrap) {
                                        delete fromWrap.dataset.inited;
                                        var lbl = fromWrap.querySelector('.searchable-dropdown-label');
                                        if (lbl) lbl.textContent = '--';
                                    }
                                    if (toWrap) {
                                        delete toWrap.dataset.inited;
                                        var lbl = toWrap.querySelector('.searchable-dropdown-label');
                                        if (lbl) lbl.textContent = '--';
                                    }
                                    if (typeof window.reinitPlanSearchableDropdowns === 'function') window.reinitPlanSearchableDropdowns();
                                    if (typeof window.savePlanRowsToStorage === 'function') window.savePlanRowsToStorage();
                                })
                                .catch(function () {
                                    fromDdl.innerHTML = '<option value="">خطأ</option>';
                                    toDdl.innerHTML = '<option value="">خطأ</option>';
                                    fromDdl.disabled = toDdl.disabled = false;
                                });
                        }
                        function parseDateYMD(str) {
                            var p = (str || '').trim().split('-');
                            if (p.length !== 3) return null;
                            var y = parseInt(p[0], 10), m = parseInt(p[1], 10) - 1, d = parseInt(p[2], 10);
                            if (isNaN(y) || isNaN(m) || isNaN(d)) return null;
                            return new Date(y, m, d);
                        }
                        
                       
                        document.addEventListener('DOMContentLoaded', function () {
                            document.body.addEventListener('change', function (e) {
                                if (e.target && e.target.classList && e.target.classList.contains('plan-surah')) {
                                    loadAyahsForRow(e.target);
                                }
                                if (e.target && e.target.classList && e.target.classList.contains('plan-from-ayah')) {
                                    var row = e.target.closest('tr');
                                    var toDdl = row ? row.querySelector('.plan-to-ayah') : null;
                                    var fromVal = parseInt(e.target.value, 10) || 0;
                                    if (toDdl && toDdl.value) {
                                        var toVal = parseInt(toDdl.value, 10) || 0;
                                        if (toVal < fromVal) {
                                            toDdl.value = e.target.value;
                                            var toWrap = toDdl.closest('.searchable-dropdown');
                                            var toLabel = toWrap ? toWrap.querySelector('.searchable-dropdown-label') : null;
                                            if (toLabel) toLabel.textContent = e.target.value;
                                        }
                                    }
                                }
                            });
                        });

                        // Searchable dropdowns: surah, from ayah, to ayah, type (trigger + panel with search + list)
                        (function () {
                            function initSearchableDropdowns() {
                                document.querySelectorAll('.searchable-dropdown').forEach(function (wrapper) {
                                    if (wrapper.dataset.inited) return;
                                    wrapper.dataset.inited = '1';
                                    var select = wrapper.querySelector('select');
                                    var trigger = wrapper.querySelector('.searchable-dropdown-trigger');
                                    var label = wrapper.querySelector('.searchable-dropdown-label');
                                    if (!label && trigger) {
                                        label = document.createElement('span');
                                        label.className = 'searchable-dropdown-label';
                                        trigger.insertBefore(label, trigger.firstChild);
                                    }
                                    var panel = wrapper.querySelector('.searchable-dropdown-panel');
                                    var searchInput = wrapper.querySelector('.searchable-dropdown-search-input');
                                    var listEl = wrapper.querySelector('.searchable-dropdown-list');
                                    if (!select || !trigger || !listEl) return;

                                    var defaultEmpty = '--';
                                    if (select.classList.contains('plan-surah')) defaultEmpty = '-- اختر السورة --';

                                    function updateLabel() {
                                        var opt = select.options[select.selectedIndex];
                                        if (label) label.textContent = opt ? opt.text : defaultEmpty;
                                    }
                                    function buildList() {
                                        listEl.innerHTML = '';
                                        var fromAyahMin = null;
                                        if (select.classList.contains('plan-to-ayah')) {
                                            var row = select.closest('tr');
                                            var fromDdl = row ? row.querySelector('.plan-from-ayah') : null;
                                            fromAyahMin = fromDdl && fromDdl.value ? parseInt(fromDdl.value, 10) : null;
                                        }
                                        for (var i = 0; i < select.options.length; i++) {
                                            var opt = select.options[i];
                                            var val = opt.value || '';
                                            if (fromAyahMin != null && val !== '') {
                                                var num = parseInt(val, 10);
                                                if (num < fromAyahMin) continue;
                                            }
                                            var div = document.createElement('div');
                                            div.className = 'searchable-dropdown-option';
                                            div.setAttribute('data-value', val);
                                            div.setAttribute('data-name', (opt.text || '').trim());
                                            div.textContent = opt.text || '';
                                            listEl.appendChild(div);
                                        }
                                        listEl.querySelectorAll('.searchable-dropdown-option').forEach(function (opt) {
                                            opt.addEventListener('click', function () {
                                                var val = opt.getAttribute('data-value') || '';
                                                select.value = val;
                                                updateLabel();
                                                wrapper.classList.remove('open');
                                                if (select.classList.contains('plan-surah')) {
                                                    var ev = document.createEvent('HTMLEvents');
                                                    ev.initEvent('change', true, false);
                                                    select.dispatchEvent(ev);
                                                }
                                                if (select.classList.contains('plan-type')) {
                                                    var ev = document.createEvent('HTMLEvents');
                                                    ev.initEvent('change', true, false);
                                                    select.dispatchEvent(ev);
                                                }
                                                if (wrapper.closest('.plan-row') && typeof window.savePlanRowsToStorage === 'function') window.savePlanRowsToStorage();
                                            });
                                        });
                                    }
                                    function filterList() {
                                        var q = (searchInput ? searchInput.value : '').trim().toLowerCase();
                                        listEl.querySelectorAll('.searchable-dropdown-option').forEach(function (opt) {
                                            var name = (opt.getAttribute('data-name') || '').toLowerCase();
                                            opt.classList.toggle('hide-by-search', q ? name.indexOf(q) === -1 : false);
                                        });
                                    }
                                    buildList();
                                    updateLabel();

                                    trigger.addEventListener('click', function (e) {
                                        e.preventDefault();
                                        wrapper.classList.toggle('open');
                                        if (wrapper.classList.contains('open')) {
                                            buildList();
                                            filterList();
                                            if (searchInput) { searchInput.value = ''; filterList(); searchInput.focus(); }
                                        }
                                    });
                                    if (searchInput) searchInput.addEventListener('input', filterList);
                                    document.body.addEventListener('click', function (e) {
                                        if (wrapper.classList.contains('open') && !wrapper.contains(e.target)) wrapper.classList.remove('open');
                                    });
                                });
                            }
                            window.reinitPlanSearchableDropdowns = initSearchableDropdowns;
                            document.addEventListener('DOMContentLoaded', function () { initSearchableDropdowns(); });

                            if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                                    initSearchableDropdowns();
                                });
                            }
                        })();

                        // Students dropdown: toggle, search, update label and selected list
                        (function () {
                            function updateStudentsLabel() {
                                var trigger = document.getElementById('studentsDropdownTrigger');
                                var label = document.getElementById('studentsDropdownLabel');
                                var listEl = document.getElementById('selectedStudentsList');
                                if (!trigger || !label) return;
                                var opts = document.querySelectorAll('.students-dropdown-option');
                                var selected = [];
                                for (var i = 0; i < opts.length; i++) {
                                    var cb = opts[i].querySelector('.student-cb');
                                    if (cb && cb.checked) {
                                        selected.push({ id: opts[i].getAttribute('data-id'), name: (opts[i].getAttribute('data-name') || '').trim() });
                                    }
                                }
                                var idsInput = document.getElementById('<%= SelectedStudentIds.ClientID %>');
                                if (idsInput) idsInput.value = selected.length ? selected.map(function (s) { return s.id; }).join(',') : '';
                                if (selected.length === 0) {
                                    label.textContent = 'اختر الطلاب';
                                    if (listEl) listEl.innerHTML = '<span class="selected-students-empty" id="selectedStudentsEmpty">لم يتم اختيار طلاب</span>';
                                } else {
                                    label.textContent = selected.length === 1 ? (selected[0].name || 'طالب واحد') : selected.length + ' طلاب محددون';
                                    if (listEl) {
                                        listEl.innerHTML = selected.map(function (s) {
                                            var name = (s.name || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
                                            var id = (s.id || '').replace(/"/g, '&quot;');
                                            return '<span class="selected-student-tag" data-id="' + id + '">' +
                                                '<span>' + name + '</span>' +
                                                '<i class="fas fa-times tag-remove" title="إلغاء التحديد"></i></span>';
                                        }).join('');
                                        listEl.querySelectorAll('.tag-remove').forEach(function (btn) {
                                            btn.addEventListener('click', function (e) {
                                                e.stopPropagation();
                                                var tag = btn.closest('.selected-student-tag');
                                                var id = tag ? tag.getAttribute('data-id') : '';
                                                var opt = document.querySelector('.students-dropdown-option[data-id="' + id + '"]');
                                                var cb = opt ? opt.querySelector('.student-cb') : null;
                                                if (cb) { cb.checked = false; updateStudentsLabel(); }
                                            });
                                        });
                                    }
                                }
                            }
                            function filterByCircle() {
                                var circleSelect = document.querySelector('.circle-dropdown-plan');
                                var circleVal = circleSelect ? (circleSelect.value || '').trim() : '';
                                var opts = document.querySelectorAll('.students-dropdown-option');
                                for (var i = 0; i < opts.length; i++) {
                                    var cid = (opts[i].getAttribute('data-circle-id') || '').trim();
                                    var show = !circleVal || cid === circleVal;
                                    opts[i].classList.toggle('hide-by-circle', !show);
                                }
                            }
                            function filterBySearch() {
                                var q = (document.getElementById('studentsSearchInput').value || '').trim().toLowerCase();
                                var opts = document.querySelectorAll('.students-dropdown-option');
                                for (var i = 0; i < opts.length; i++) {
                                    if (opts[i].classList.contains('hide-by-circle')) { opts[i].classList.add('hide-by-search'); continue; }
                                    var name = (opts[i].getAttribute('data-name') || '').toLowerCase();
                                    var show = !q || name.indexOf(q) !== -1;
                                    opts[i].classList.toggle('hide-by-search', !show);
                                }
                            }
                            function selectAllVisible() {
                                var opts = document.querySelectorAll('.students-dropdown-option');
                                for (var i = 0; i < opts.length; i++) {
                                    if (opts[i].classList.contains('hide-by-circle') || opts[i].classList.contains('hide-by-search')) continue;
                                    var cb = opts[i].querySelector('.student-cb');
                                    if (cb) cb.checked = true;
                                }
                                updateStudentsLabel();
                            }
                            function deselectAllVisible() {
                                var opts = document.querySelectorAll('.students-dropdown-option');
                                for (var i = 0; i < opts.length; i++) {
                                    if (opts[i].classList.contains('hide-by-circle') || opts[i].classList.contains('hide-by-search')) continue;
                                    var cb = opts[i].querySelector('.student-cb');
                                    if (cb) cb.checked = false;
                                }
                                updateStudentsLabel();
                            }
                            function initCircleDropdown() {
                                var circleSelect = document.querySelector('.circle-dropdown-plan');
                                var circleWrap = document.getElementById('circleDropdown');
                                var circleTrigger = document.getElementById('circleDropdownTrigger');
                                var circlePanel = document.getElementById('circleDropdownPanel');
                                var circleList = document.getElementById('circleDropdownList');
                                var circleSearch = document.getElementById('circleSearchInput');
                                var circleLabel = document.getElementById('circleDropdownLabel');
                                if (!circleSelect || !circleWrap || !circleList || !circleLabel) return;
                                function buildCircleList() {
                                    circleList.innerHTML = '';
                                    for (var i = 0; i < circleSelect.options.length; i++) {
                                        var opt = circleSelect.options[i];
                                        var div = document.createElement('div');
                                        div.className = 'circle-dropdown-option';
                                        div.setAttribute('data-value', opt.value || '');
                                        div.setAttribute('data-name', (opt.text || '').trim());
                                        div.textContent = opt.text || '';
                                        circleList.appendChild(div);
                                    }
                                    circleList.querySelectorAll('.circle-dropdown-option').forEach(function (opt) {
                                        opt.addEventListener('click', function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();
                                            var val = opt.getAttribute('data-value') || '';
                                            circleSelect.value = val;
                                            circleLabel.textContent = opt.getAttribute('data-name') || 'كل الطلاب';
                                            circleWrap.classList.remove('open');
                                            filterByCircle();
                                            filterBySearch();
                                        });
                                    });
                                }
                                function filterCircleList() {
                                    var q = (circleSearch ? circleSearch.value : '').trim().toLowerCase();
                                    circleList.querySelectorAll('.circle-dropdown-option').forEach(function (opt) {
                                        var name = (opt.getAttribute('data-name') || '').toLowerCase();
                                        opt.classList.toggle('hide-by-search', q ? name.indexOf(q) === -1 : false);
                                    });
                                }
                                buildCircleList();
                                var selOpt = circleSelect.options[circleSelect.selectedIndex];
                                circleLabel.textContent = selOpt ? selOpt.text : 'كل الطلاب';
                                if (circleTrigger) {
                                    circleTrigger.addEventListener('click', function (e) {
                                        e.preventDefault();
                                        circleWrap.classList.toggle('open');
                                        if (circleWrap.classList.contains('open')) { buildCircleList(); filterCircleList(); if (circleSearch) { circleSearch.value = ''; filterCircleList(); circleSearch.focus(); } }
                                    });
                                }
                                if (circleSearch) circleSearch.addEventListener('input', filterCircleList);
                                document.body.addEventListener('click', function (e) {
                                    if (circleWrap.classList.contains('open') && !circleWrap.contains(e.target)) circleWrap.classList.remove('open');
                                });
                            }
                            document.addEventListener('DOMContentLoaded', function () {
                                initPlanStatusAndChart();
                                initCircleDropdown();
                                var dd = document.getElementById('studentsDropdown');
                                var trigger = document.getElementById('studentsDropdownTrigger');
                                var panel = document.getElementById('studentsDropdownPanel');
                                var searchInput = document.getElementById('studentsSearchInput');
                                var circleSelect = document.querySelector('.circle-dropdown-plan');
                                if (!dd || !trigger || !panel) { return; }
                                if (circleSelect) filterByCircle();
                                var selectAllBtn = document.getElementById('studentsSelectAllBtn');
                                var deselectAllBtn = document.getElementById('studentsDeselectAllBtn');
                                if (selectAllBtn) selectAllBtn.addEventListener('click', function (e) { e.preventDefault(); e.stopPropagation(); selectAllVisible(); });
                                if (deselectAllBtn) deselectAllBtn.addEventListener('click', function (e) { e.preventDefault(); e.stopPropagation(); deselectAllVisible(); });
                                trigger.addEventListener('click', function (e) {
                                    e.preventDefault();
                                    dd.classList.toggle('open');
                                    if (dd.classList.contains('open')) { filterByCircle(); filterBySearch(); searchInput.focus(); }
                                });
                                searchInput.addEventListener('input', filterBySearch);
                                searchInput.addEventListener('keydown', function (e) { if (e.key === 'Escape') { dd.classList.remove('open'); } });
                                document.body.addEventListener('click', function (e) {
                                    if (dd.classList.contains('open') && !dd.contains(e.target)) dd.classList.remove('open');
                                });
                                dd.querySelectorAll('.students-dropdown-option').forEach(function (opt) {
                                    opt.addEventListener('click', function (e) {
                                        e.preventDefault();
                                        e.stopPropagation();
                                        var cb = opt.querySelector('.student-cb');
                                        if (cb) {
                                            cb.checked = !cb.checked;
                                            updateStudentsLabel();
                                        }
                                    });
                                });
                                updateStudentsLabel();
                            });
                        })();
                    })();
                    function initPlanStatusAndChart() {
                        var progressEl = document.getElementById('progressDataJson');
                        var progress = { passed: 0, failed: 0, pending: 0, retake: 0, total: 0, daysRemaining: 0, totalPlanDays: 1, daysElapsed: 0, progressPercent: 0 };
                        if (progressEl && progressEl.textContent) {
                            try { progress = JSON.parse(progressEl.textContent.trim()); } catch (e) { }
                        }
                        // لم يعد هناك إظهار تدريجي على مستوى الواجهة؛ المنطق أصبح من قاعدة البيانات
                        var chartInstance = null;
                        var daysChartInstance = null;
                        var percentChartInstance = null;
                        var canvas = document.getElementById('planProgressChart');
                        if (typeof Chart !== 'undefined' && canvas) {
                            var ctx = canvas.getContext('2d');
                            chartInstance = new Chart(ctx, {
                                type: 'doughnut',
                                data: {
                                    labels: ['تم', 'لم يتم', 'قيد الانتظار', 'اعادة تسميع'],
                                    datasets: [{
                                        data: [progress.passed || 0, progress.failed || 0, progress.pending || 0, progress.retake || 0],
                                        backgroundColor: ['#28a745', '#dc3545', '#6c757d', '#fd7e14'],
                                        borderWidth: 1
                                    }]
                                },
                                options: { responsive: true, maintainAspectRatio: true }
                            });
                        }
                        var daysCanvas = document.getElementById('planDaysChart');
                        if (typeof Chart !== 'undefined' && daysCanvas) {
                            var daysRem = Math.max(0, progress.daysRemaining || 0);
                            var daysEl = Math.max(0, progress.daysElapsed || 0);
                            if (daysRem === 0 && daysEl === 0) daysEl = 1;
                            daysChartInstance = new Chart(daysCanvas.getContext('2d'), {
                                type: 'doughnut',
                                data: {
                                    labels: ['أيام متبقية', 'أيام منقضية'],
                                    datasets: [{
                                        data: [daysRem, daysEl],
                                        backgroundColor: ['#17a2b8', '#e9ecef'],
                                        borderWidth: 1
                                    }]
                                },
                                options: { responsive: true, maintainAspectRatio: true }
                            });
                        }
                        var percentCanvas = document.getElementById('planPercentChart');
                        if (typeof Chart !== 'undefined' && percentCanvas) {
                            var pct = Math.min(100, Math.max(0, progress.progressPercent || 0));
                            percentChartInstance = new Chart(percentCanvas.getContext('2d'), {
                                type: 'doughnut',
                                data: {
                                    labels: ['منجز %', 'متبقي %'],
                                    datasets: [{
                                        data: [pct, 100 - pct],
                                        backgroundColor: ['#28a745', '#f8f9fa'],
                                        borderWidth: 1
                                    }]
                                },
                                options: { responsive: true, maintainAspectRatio: true }
                            });
                        }
                        function getStatusLabels(row) {
                            if (row) {
                                if (row.closest('#tab-tathbit')) return { pass: 'تم التثبيت', fail: 'لم يتم التثبيت', retake: null };
                                if (row.closest('#tab-revise')) return { pass: 'تم المراجعة', fail: 'لم يتم المراجعة', retake: 'اعادة مراجعة' };
                                return { pass: 'تم الحفظ', fail: 'لم يتم الحفظ', retake: 'اعادة تسميع' };
                            }
                            return { pass: 'تم الحفظ', fail: 'لم يتم الحفظ', retake: 'اعادة تسميع' };
                        }
                        function applyStatusLabelsToRow(row) {
                            if (!row) return;
                            var labels = getStatusLabels(row);
                            var passBtn = row.querySelector('.plan-status-pass');
                            var failBtn = row.querySelector('.plan-status-fail');
                            var retakeBtn = row.querySelector('.plan-status-retake');
                            if (passBtn) passBtn.textContent = labels.pass;
                            if (failBtn) failBtn.textContent = labels.fail;
                            if (retakeBtn && labels.retake) retakeBtn.textContent = labels.retake;
                        }
                        function syncRowButtonActive(row) {
                            if (!row) return;
                            var status = (row.getAttribute('data-status') || '').trim();
                            row.querySelectorAll('.plan-status-btn').forEach(function (btn) {
                                var s = (btn.getAttribute('data-status') || '').trim();
                                btn.classList.toggle('active', s === status);
                            });
                        }
                        document.querySelectorAll('.plan-status-row').forEach(function (row) {
                            syncRowButtonActive(row);
                            applyStatusLabelsToRow(row);
                        });
                        document.body.addEventListener('change', function (e) {
                            if (e.target && e.target.classList && e.target.classList.contains('plan-type')) {
                                var row = e.target.closest('.plan-status-row');
                                if (row) {
                                    row.setAttribute('data-plan-type', e.target.value || '');
                                    applyStatusLabelsToRow(row);
                                }
                            }
                        });
                        document.addEventListener('click', function (e) {
                            var btn = e.target && e.target.closest ? e.target.closest('.plan-status-btn') : null;
                            if (!btn) return;
                            e.preventDefault();
                            var row = btn.closest('.plan-status-row');
                            if (!row) return;
                            var rowKey = row.getAttribute('data-key');
                            var status = (btn.getAttribute('data-status') || '').trim();
                            var tabType = (row.getAttribute('data-tab-type') || '').trim();
                            var oldStatus = (row.getAttribute('data-status') || '').trim();
                            if (!rowKey || !status) return;
                            var hid = document.getElementById('<%= HiddenViewStudentId.ClientID %>');
                            var studentId = hid ? hid.value : '';
                            if (!studentId) {
                                console.error('[StudentPlan] Cannot change status: HiddenViewStudentId not found or empty.');
                                return;
                            }
                            var url = 'StudentPlan2.aspx/LogPlanRowStatus?studentId=' + encodeURIComponent(studentId);
                            var xhr = new XMLHttpRequest();
                            xhr.open('POST', url, true);
                            xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
                            // Ensure we send all parameters matching the new C# signature
                            var payload = JSON.stringify({
                                rowKey: rowKey,
                                status: status,
                                tabType: tabType
                            });
                            xhr.onreadystatechange = function () {
                                if (xhr.readyState !== 4) return;
                                var resp = null;
                                try {
                                    if (xhr.status !== 200) {
                                        console.error('[StudentPlan] LogPlanRowStatus HTTP error:', xhr.status, xhr.statusText, xhr.responseText ? xhr.responseText.substring(0, 500) : '');
                                        return;
                                    }
                                    resp = JSON.parse(xhr.responseText);
                                } catch (ex) {
                                    console.error('[StudentPlan] LogPlanRowStatus parse error:', ex.message, 'Response:', xhr.responseText ? xhr.responseText.substring(0, 300) : '');
                                    return;
                                }
                                var d = resp && resp.d != null ? resp.d : resp;
                                if (!d) {
                                    console.error('[StudentPlan] LogPlanRowStatus empty response:', resp);
                                    return;
                                }
                                if (!d.success) {
                                    console.error('[StudentPlan] LogPlanRowStatus server returned success=false', d);
                                    return;
                                }
                                try {
                                    var keyAttr = (rowKey || '').replace(/"/g, '\\"');
                                    document.querySelectorAll('.plan-status-row[data-key="' + keyAttr + '"]').forEach(function (r) {
                                        r.setAttribute('data-status', status);
                                        syncRowButtonActive(r);
                                        applyStatusLabelsToRow(r);
                                    });
                                    if (d.passed !== undefined && chartInstance && chartInstance.data && chartInstance.data.datasets && chartInstance.data.datasets[0]) {
                                        chartInstance.data.datasets[0].data = [d.passed || 0, d.failed || 0, d.pending || 0, d.retake || 0];
                                        chartInstance.update('none');
                                    }
                                    if (d.passed !== undefined && percentChartInstance && percentChartInstance.data && percentChartInstance.data.datasets && percentChartInstance.data.datasets[0] && d.total > 0) {
                                        var pct = Math.round(100 * (d.passed || 0) / d.total);
                                        percentChartInstance.data.datasets[0].data = [pct, 100 - pct];
                                        percentChartInstance.update('none');
                                    }
                                    // Memorizing: تم الحفظ → go to tathbit tab (redirect with tab param)
                                    var isMemorizingPass = (tabType === 'حفظ' && rowKey.indexOf('memorizing_') === 0 && status === 'تم');
                                    if (isMemorizingPass) {
                                        var url = new URL(window.location.href);
                                        url.searchParams.set('tab', 'tab-tathbit');
                                        try { window.location = url.toString(); return; } catch (e) { }
                                    }
                                    // Revise tab with تم/لم يتم/اعادة تسميع: reload to show next pending record
                                    var isReviseStatusChange = (tabType === 'مراجعة' && rowKey.indexOf('revise_') === 0 && ['تم','لم يتم','اعادة تسميع'].indexOf(status) >= 0);
                                    if (isReviseStatusChange) {
                                        try { location.reload(); return; } catch (e) { }
                                    }
                                    var skipReload = false;
                                    if (!skipReload) {
                                        if (!window._statusAdvanceKeys) window._statusAdvanceKeys = {};
                                        var advanceKey = rowKey + '|' + (oldStatus || '');
                                        if (!window._statusAdvanceKeys[advanceKey]) {
                                            window._statusAdvanceKeys[advanceKey] = true;
                                            try { location.reload(); return; } catch (e) { }
                                        }
                                    }
                                    var progressEl = document.getElementById('progressDataJson');
                                    if (progressEl && d.passed !== undefined && d.total !== undefined) {
                                        var pct = d.total > 0 ? Math.round(100 * (d.passed || 0) / d.total) : 0;
                                        var existing = {};
                                        try { existing = JSON.parse(progressEl.textContent.trim() || '{}'); } catch (e2) { }
                                        progressEl.textContent = JSON.stringify({
                                            passed: d.passed || 0,
                                            failed: d.failed || 0,
                                            pending: d.pending != null ? d.pending : 0,
                                            retake: d.retake != null ? d.retake : 0,
                                            total: d.total,
                                            daysRemaining: existing.daysRemaining,
                                            totalPlanDays: existing.totalPlanDays,
                                            daysElapsed: existing.daysElapsed,
                                            progressPercent: pct
                                        });
                                    }
                                    if (d.logRow) {
                                        var tbody = document.querySelector('.assessment-log-table tbody');
                                        var emptyPanel = document.querySelector('[id*="PanelAssessmentLogEmpty"]');
                                        if (tbody) {
                                            if (emptyPanel) emptyPanel.style.display = 'none';
                                            function esc(s) { return (s || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;'); }
                                            var tr = document.createElement('tr');
                                            tr.innerHTML = '<td>' + esc(d.logRow.rowLabel) + '</td><td>' + esc(d.logRow.statusDisplay) + '</td><td>' + esc(d.logRow.teacherName) + '</td><td>' + esc(d.logRow.loggedAtFormatted) + '</td>';
                                            tbody.insertBefore(tr, tbody.firstChild);
                                        }
                                    }
                                    // Revise tab: replace row with next pending record (or show empty message)
                                    var isReviseStatus = ['تم','لم يتم','اعادة تسميع'].indexOf(status) >= 0;
                                    if (tabType === 'مراجعة' && rowKey.indexOf('revise_') === 0 && isReviseStatus) {
                                        var reviseTbody = document.querySelector('#tab-revise .plan-table tbody');
                                        if (row && row.parentNode) row.remove();
                                        function esc(s) { return (s || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;'); }
                                        if (d.nextReviseRecord) {
                                            var nr = d.nextReviseRecord;
                                            var qurl = 'Quran.aspx?surahId=' + (nr.surahId || '') + '&from=' + (nr.fromAyah || '') + '&to=' + (nr.toAyah || '');
                                            var tr = document.createElement('tr');
                                            tr.className = 'plan-item-row plan-status-row';
                                            tr.setAttribute('data-key', nr.key || '');
                                            tr.setAttribute('data-status', 'قيد الانتظار');
                                            tr.setAttribute('data-plan-type', 'مراجعة');
                                            tr.setAttribute('data-tab-type', 'مراجعة');
                                            tr.innerHTML = '<td>' + esc(nr.surahName) + '</td><td>' + (nr.fromAyah || '') + '</td><td>' + (nr.toAyah || '') + '</td>' +
                                                '<td class="plan-status-cell"><button type="button" class="plan-status-btn plan-status-pass" data-status="تم">تم المراجعة</button><button type="button" class="plan-status-btn plan-status-fail" data-status="لم يتم">لم يتم المراجعة</button><button type="button" class="plan-status-btn plan-status-retake" data-status="اعادة تسميع">اعادة مراجعة</button><button type="button" class="plan-status-btn plan-status-pending" data-status="قيد الانتظار">قيد الانتظار</button></td>' +
                                                '<td><a href="' + esc(qurl) + '" target="_blank" class="btn-quran">المصحف الالكتروني</a></td>';
                                            if (reviseTbody) reviseTbody.appendChild(tr);
                                            applyStatusLabelsToRow(tr);
                                            syncRowButtonActive(tr);
                                        } else {
                                            var emptyTr = document.createElement('tr');
                                            emptyTr.innerHTML = '<td colspan="5" style="text-align:center; padding:20px; color:#6c757d;">لا توجد مراجعة قادمة.</td>';
                                            if (reviseTbody) reviseTbody.appendChild(emptyTr);
                                        }
                                    }
                                } catch (err) {
                                    console.error('[StudentPlan] Error applying status update:', err);
                                }
                            };
                            xhr.onerror = function () {
                                console.error('[StudentPlan] LogPlanRowStatus network error (XHR onerror).');
                            };
                            try {
                                xhr.send(payload);
                            } catch (sendErr) {
                                console.error('[StudentPlan] LogPlanRowStatus send error:', sendErr);
                            }
                        });
                    }

                    // Next Memorize Modal
                    (function () {
                        // Create modal HTML
                        var modalHTML = `
                    <div id="nextMemorizeModal" class="modal" style="display: none; position: fixed; z-index: 1000; left: 0; top: 0; width: 100%; height: 100%; background-color: rgba(0,0,0,0.5);">
                        <div class="modal-content" style="background-color: #fefefe; margin: 10% auto; padding: 30px; border-radius: 12px; width: 400px; box-shadow: 0 4px 20px rgba(0,0,0,0.2);">
                            <div style="text-align: center; margin-bottom: 25px;">
                                <h3 style="color: #28a745; margin: 0 0 10px 0;">الحفظ القادم</h3>
                                <p style="color: #666; margin: 0;">اختر تاريخ الحفظ القادم للبنود المحددة</p>
                            </div>
                            
                            <div style="margin-bottom: 25px;">
                                <label style="display: block; margin-bottom: 8px; font-weight: 600; color: #333;">تاريخ الحفظ القادم</label>
                                <input type="date" id="nextMemorizeDate" style="width: 100%; padding: 10px; border: 2px solid #ddd; border-radius: 8px; font-family: Cairo, sans-serif; font-size: 1rem;" />
                            </div>
                            
                            <div style="display: flex; justify-content: center; gap: 15px;">
                                <button type="button" id="confirmNextMemorizeBtn" style="padding: 12px 30px; border-radius: 25px; font-weight: 600; border: none; background: linear-gradient(135deg, #28a745, #20c997); color: white; cursor: pointer; transition: all 0.3s;">
                                    تأكيد
                                </button>
                                <button type="button" id="cancelNextMemorizeBtn" style="padding: 12px 30px; border-radius: 25px; font-weight: 600; border: 2px solid #ddd; background: white; color: #666; cursor: pointer; transition: all 0.3s;">
                                    إلغاء
                                </button>
                            </div>
                        </div>
                    </div>
                `;

                        // Add modal to body
                        document.body.insertAdjacentHTML('beforeend', modalHTML);

                        var modal = document.getElementById('nextMemorizeModal');
                        //var nextMemorizeBtn = document.getElementById('NextMemorizeBtn');
                        var confirmBtn = document.getElementById('confirmNextMemorizeBtn');
                        var cancelBtn = document.getElementById('cancelNextMemorizeBtn');
                        var dateInput = document.getElementById('nextMemorizeDate');

                        // Set default date to tomorrow
                        var tomorrow = new Date();
                        tomorrow.setDate(tomorrow.getDate() + 1);
                        dateInput.valueAsDate = tomorrow;

                        // Show/hide next memorize button based on checkbox selection
                        function updateNextMemorizeButton() {
                            var checkboxes = document.querySelectorAll('.plan-item-checkbox:checked');
                            //nextMemorizeBtn.style.display = checkboxes.length > 0 ? 'inline-block' : 'none';
                        }

                        // Select all checkboxes functionality
                        var selectAllCheckbox = document.getElementById('selectAllPlanItems');
                        if (selectAllCheckbox) {
                            selectAllCheckbox.addEventListener('change', function () {
                                var checkboxes = document.querySelectorAll('.plan-item-checkbox');
                                checkboxes.forEach(function (cb) {
                                    cb.checked = selectAllCheckbox.checked;
                                });
                                updateNextMemorizeButton();
                            });
                        }

                        // Individual checkbox change event
                        document.addEventListener('change', function (e) {
                            if (e.target.classList.contains('plan-item-checkbox')) {
                                updateNextMemorizeButton();
                            }
                        });

                        // Show modal when next memorize button is clicked
                        //nextMemorizeBtn.addEventListener('click', function () {
                        //    var selectedItems = [];
                        //    document.querySelectorAll('.plan-item-checkbox:checked').forEach(function (cb) {
                        //        selectedItems.push(cb.getAttribute('data-key'));
                        //    });

                        //    if (selectedItems.length === 0) {
                        //        alert('يرجى تحديد بنود الخطة أولاً');
                        //        return;
                        //    }

                        //    modal.style.display = 'block';
                        //});

                        // Close modal when cancel button is clicked
                        cancelBtn.addEventListener('click', function () {
                            modal.style.display = 'none';
                        });

                        // Close modal when clicking outside
                        window.addEventListener('click', function (e) {
                            if (e.target === modal) {
                                modal.style.display = 'none';
                            }
                        });

                        // Confirm button click - send to server
                        confirmBtn.addEventListener('click', function () {
                            var selectedDate = dateInput.value;
                            if (!selectedDate) {
                                alert('يرجى اختيار تاريخ');
                                return;
                            }

                            var selectedItems = [];
                            document.querySelectorAll('.plan-item-checkbox:checked').forEach(function (cb) {
                                selectedItems.push(cb.getAttribute('data-key'));
                            });

                            // Send to server
                            saveNextMemorizeDate(selectedItems, selectedDate);
                        });

                        function saveNextMemorizeDate(itemKeys, date) {
                            var hid = document.getElementById('<%= HiddenViewStudentId.ClientID %>');
                            var studentId = hid ? hid.value : '';
                            if (!studentId) {
                                alert('Invalid student ID');
                                return;
                            }

                            var xhr = new XMLHttpRequest();
                            xhr.open('POST', 'StudentPlan2.aspx/SaveNextMemorizeDate?studentId=' + encodeURIComponent(studentId), true);
                            xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');

                            xhr.onload = function () {
                                if (xhr.status === 200) {
                                    var response = JSON.parse(xhr.responseText);
                                    if (response.d && response.d.success) {
                                        alert('تم حفظ تاريخ الحفظ القادم بنجاح');
                                        modal.style.display = 'none';

                                        // Clear selection
                                        document.querySelectorAll('.plan-item-checkbox:checked').forEach(function (cb) {
                                            cb.checked = false;
                                        });
                                        if (selectAllCheckbox) {
                                            selectAllCheckbox.checked = false;
                                        }
                                        updateNextMemorizeButton();

                                        // Refresh page to show updated dates
                                        setTimeout(function () {
                                            location.reload();
                                        }, 1000);
                                    } else {
                                        alert('حدث خطأ أثناء الحفظ: ' + (response.d.message || ''));
                                    }
                                } else {
                                    alert('حدث خطأ في الاتصال بالخادم');
                                }
                            };

                            xhr.onerror = function () {
                                alert('حدث خطأ في الاتصال بالخادم');
                            };

                            xhr.send(JSON.stringify({
                                itemKeys: itemKeys,
                                date: date
                            }));
                        }

                        // Next Revise Modal
                        var modalReviseHTML = `
                    <div id="nextReviseModal" class="modal" style="display: none; position: fixed; z-index: 1000; left: 0; top: 0; width: 100%; height: 100%; background-color: rgba(0,0,0,0.5);">
                        <div class="modal-content" style="background-color: #fefefe; margin: 10% auto; padding: 30px; border-radius: 12px; width: 400px; box-shadow: 0 4px 20px rgba(0,0,0,0.2);">
                            <div style="text-align: center; margin-bottom: 25px;">
                                <h3 style="color: #007bff; margin: 0 0 10px 0;">المراجعة القادمة</h3>
                                <p style="color: #666; margin: 0;">اختر تاريخ المراجعة القادمة للبنود المحددة</p>
                            </div>
                            
                            <div style="margin-bottom: 25px;">
                                <label style="display: block; margin-bottom: 8px; font-weight: 600; color: #333;">تاريخ المراجعة القادمة</label>
                                <input type="date" id="nextReviseDate" style="width: 100%; padding: 10px; border: 2px solid #ddd; border-radius: 8px; font-family: Cairo, sans-serif; font-size: 1rem;" />
                            </div>
                            
                            <div style="display: flex; justify-content: center; gap: 15px;">
                                <button type="button" id="confirmNextReviseBtn" style="padding: 12px 30px; border-radius: 25px; font-weight: 600; border: none; background: linear-gradient(135deg, #007bff, #0056b3); color: white; cursor: pointer; transition: all 0.3s;">
                                    تأكيد
                                </button>
                                <button type="button" id="cancelNextReviseBtn" style="padding: 12px 30px; border-radius: 25px; font-weight: 600; border: 2px solid #ddd; background: white; color: #666; cursor: pointer; transition: all 0.3s;">
                                    إلغاء
                                </button>
                            </div>
                        </div>
                    </div>
                `;

                        document.body.insertAdjacentHTML('beforeend', modalReviseHTML);

                        //var nextReviseBtn = document.getElementById('NextReviseBtn');
                        var confirmReviseBtn = document.getElementById('confirmNextReviseBtn');
                        var cancelReviseBtn = document.getElementById('cancelNextReviseBtn');
                        var reviseModal = document.getElementById('nextReviseModal');
                        var reviseDateInput = document.getElementById('nextReviseDate');

                        var tomorrowRevise = new Date();
                        tomorrowRevise.setDate(tomorrowRevise.getDate() + 1);
                        reviseDateInput.valueAsDate = tomorrowRevise;

                        function updateNextReviseButton() {
                            var checkboxes = document.querySelectorAll('.plan-item-checkbox:checked');
                            //nextReviseBtn.style.display = checkboxes.length > 0 ? 'inline-block' : 'none';
                        }

                        //nextReviseBtn.addEventListener('click', function () {
                        //    var selectedItems = [];
                        //    document.querySelectorAll('.plan-item-checkbox:checked').forEach(function (cb) {
                        //        selectedItems.push(cb.getAttribute('data-key'));
                        //    });

                        //    if (selectedItems.length === 0) {
                        //        alert('يرجى تحديد بنود الخطة أولاً');
                        //        return;
                        //    }

                        //    reviseModal.style.display = 'block';
                        //});

                        cancelReviseBtn.addEventListener('click', function () {
                            reviseModal.style.display = 'none';
                        });

                        window.addEventListener('click', function (e) {
                            if (e.target === reviseModal) {
                                reviseModal.style.display = 'none';
                            }
                        });

                        confirmReviseBtn.addEventListener('click', function () {
                            var selectedDate = reviseDateInput.value;
                            if (!selectedDate) {
                                alert('يرجى اختيار تاريخ');
                                return;
                            }

                            var selectedItems = [];
                            document.querySelectorAll('.plan-item-checkbox:checked').forEach(function (cb) {
                                selectedItems.push(cb.getAttribute('data-key'));
                            });

                            saveNextReviseDate(selectedItems, selectedDate);
                        });

                        function saveNextReviseDate(itemKeys, date) {
                            var hid = document.getElementById('<%= HiddenViewStudentId.ClientID %>');
                            var studentId = hid ? hid.value : '';
                            if (!studentId) {
                                alert('Invalid student ID');
                                return;
                            }

                            var xhr = new XMLHttpRequest();
                            xhr.open('POST', 'StudentPlan2.aspx/SaveNextReviseDate?studentId=' + encodeURIComponent(studentId), true);
                            xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');

                            xhr.onload = function () {
                                if (xhr.status === 200) {
                                    var response = JSON.parse(xhr.responseText);
                                    if (response.d && response.d.success) {
                                        alert('تم حفظ تاريخ المراجعة القادمة بنجاح');
                                        reviseModal.style.display = 'none';

                                        document.querySelectorAll('.plan-item-checkbox:checked').forEach(function (cb) {
                                            cb.checked = false;
                                        });
                                        if (selectAllCheckbox) {
                                            selectAllCheckbox.checked = false;
                                        }
                                        updateNextReviseButton();

                                        setTimeout(function () {
                                            location.reload();
                                        }, 1000);
                                    } else {
                                        alert('حدث خطأ أثناء الحفظ: ' + (response.d.message || ''));
                                    }
                                } else {
                                    alert('حدث خطأ في الاتصال بالخادم');
                                }
                            };

                            xhr.onerror = function () {
                                alert('حدث خطأ في الاتصال بالخادم');
                            };

                            xhr.send(JSON.stringify({
                                itemKeys: itemKeys,
                                date: date
                            }));
                        }
                    })();

                </script>
            </asp:Content>