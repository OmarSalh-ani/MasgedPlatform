import { createBrowserRouter, Navigate } from 'react-router-dom'
import { ProtectedRoute } from '@/components/auth/ProtectedRoute'
import { SetupGuard } from '@/components/auth/SetupGuard'
import { AdminLayout } from '@/layouts/AdminLayout'
import { AboutEditPage } from '@/pages/about/AboutEditPage'
import { ActivitiesPage } from '@/pages/activities/ActivitiesPage'
import { ActivityFormPage } from '@/pages/activities/ActivityFormPage'
import { CompetitionFormPage } from '@/pages/competitions/CompetitionFormPage'
import { CompetitionsPage } from '@/pages/competitions/CompetitionsPage'
import { ContactInfoFormPage } from '@/pages/contact-info/ContactInfoFormPage'
import { ContactInfoPage } from '@/pages/contact-info/ContactInfoPage'
import { SocialLinkFormPage } from '@/pages/social-links/SocialLinkFormPage'
import { SocialLinksPage } from '@/pages/social-links/SocialLinksPage'
import { HeroSlideFormPage } from '@/pages/hero-slides/HeroSlideFormPage'
import { HeroSlidesPage } from '@/pages/hero-slides/HeroSlidesPage'
import { NewsFormPage } from '@/pages/news/NewsFormPage'
import { NewsPage } from '@/pages/news/NewsPage'
import { LoginPage } from '@/pages/login/LoginPage'
import { ExpensiveFormPage } from '@/pages/expensives/ExpensiveFormPage'
import { ExpensivesPage } from '@/pages/expensives/ExpensivesPage'
import { MosqueFormPage } from '@/pages/mosques/MosqueFormPage'
import { MosquesPage } from '@/pages/mosques/MosquesPage'
import { CircleFormPage } from '@/pages/circles/CircleFormPage'
import { CirclesPage } from '@/pages/circles/CirclesPage'
import { SendNoteFormPage } from '@/pages/send-notes/SendNoteFormPage'
import { SendNotesPage } from '@/pages/send-notes/SendNotesPage'
import { FileFormPage } from '@/pages/files-manager/FileFormPage'
import { FilesManagersPage } from '@/pages/files-manager/FilesManagersPage'
import { StudentCardPrintPage } from '@/pages/students/StudentCardPrintPage'
import { StudentPage } from '@/pages/students/StudentPage'
import { TeacherCardPrintPage } from '@/pages/teachers/TeacherCardPrintPage'
import { TeacherSalariesPage } from '@/pages/teacher-salaries/TeacherSalariesPage'
import { TeacherSalaryFormPage } from '@/pages/teacher-salaries/TeacherSalaryFormPage'
import { TeacherSalaryReportPage } from '@/pages/teacher-salaries/TeacherSalaryReportPage'
import { TeacherFormPage } from '@/pages/teachers/TeacherFormPage'
import { TeachersPage } from '@/pages/teachers/TeachersPage'
import { WomansActivitiesPage } from '@/pages/womans-activities/WomansActivitiesPage'
import { AttendanceReportPage } from '@/pages/attendance-report/AttendanceReportPage'
import { TeachersAttendancePage } from '@/pages/teachers-attendance/TeachersAttendancePage'
import { CurrentStudentsPlansPage } from '@/pages/current-students-plans/CurrentStudentsPlansPage'
import { HomePage } from '@/pages/home/HomePage'
import { OthaiminCenterPage } from '@/pages/OthaiminCenter/OthaiminCenterPage'
import { StudentPlanPage } from '@/pages/student-plans/StudentPlanPage'
import { ParentPanelLogStatisticsPage } from '@/pages/parent-panel-log-statistics/ParentPanelLogStatisticsPage'
import { PlanLevelPage } from '@/pages/plan-levels/PlanLevelPage'
import { MemorizationRevisionReportPage } from '@/pages/memorization-revision-report/MemorizationRevisionReportPage'
import { SpecialStudentsReportPage } from '@/pages/special-students-report/SpecialStudentsReportPage'
import { Students2Page } from '@/pages/students2/Students2Page'
import { ParentsFollowupPage } from '@/pages/parents-followup/ParentsFollowupPage'
import { QRGeneratorPage } from '@/pages/qr-generator/QRGeneratorPage'
import { StatisticsPage } from '@/pages/statistics/StatisticsPage'
import { SubscribePage } from '@/pages/subscribe/SubscribePage'
import { WhatsappPendingPage } from '@/pages/whatsapp-pending/WhatsappPendingPage'
import { WhatsappPreConfiguredPage } from '@/pages/whatsapp-config/WhatsappPreConfiguredPage'
import { WhatsappQrPage } from '@/pages/whatsapp-qr/WhatsappQrPage'
import { WhatsappSenderPage } from '@/pages/whatsapp-sender/WhatsappSenderPage'
import { PushNotificationPage } from '@/pages/push-notifications/PushNotificationPage'
import { TestCertificatePage } from '@/pages/test-certificate/TestCertificatePage'
import { TestsReportPage } from '@/pages/tests/TestsReportPage'
import { SettingsPage } from '@/pages/settings/SettingsPage'
import { IntegrationsPage } from '@/pages/integrations/IntegrationsPage'
import { SetupPage } from '@/pages/setup/SetupPage'
import { WorkDaysPage } from '@/pages/work-days/WorkDaysPage'

export const router = createBrowserRouter([
  {
    element: <SetupGuard />,
    children: [
      { path: '/setup', element: <SetupPage /> },
      { path: '/login', element: <LoginPage /> },
      { path: '/parents-followup', element: <ParentsFollowupPage /> },
      { path: '/subscribe', element: <SubscribePage /> },
      {
        element: <ProtectedRoute />,
        children: [
          { path: 'teachers/:id/card-print', element: <TeacherCardPrintPage /> },
          { path: 'students/:id/card-print', element: <StudentCardPrintPage /> },
          { path: 'test-certificate', element: <TestCertificatePage /> },
          {
            path: '/',
            element: <AdminLayout />,
            children: [
              { index: true, element: <Navigate to="/home" replace /> },
              { path: 'about', element: <AboutEditPage /> },
              { path: 'activities', element: <ActivitiesPage /> },
              { path: 'activities/new', element: <ActivityFormPage /> },
              { path: 'activities/:id/edit', element: <ActivityFormPage /> },
          { path: 'competitions', element: <CompetitionsPage /> },
          { path: 'competitions/new', element: <CompetitionFormPage /> },
          { path: 'competitions/:id/edit', element: <CompetitionFormPage /> },
          { path: 'contact-info', element: <ContactInfoPage /> },
          { path: 'contact-info/new', element: <ContactInfoFormPage /> },
          { path: 'contact-info/:id/edit', element: <ContactInfoFormPage /> },
          { path: 'social-links', element: <SocialLinksPage /> },
          { path: 'social-links/new', element: <SocialLinkFormPage /> },
          { path: 'social-links/:id/edit', element: <SocialLinkFormPage /> },
          { path: 'hero-slides', element: <HeroSlidesPage /> },
          { path: 'hero-slides/new', element: <HeroSlideFormPage /> },
          { path: 'hero-slides/:id/edit', element: <HeroSlideFormPage /> },
          { path: 'news', element: <NewsPage /> },
          { path: 'news/new', element: <NewsFormPage /> },
          { path: 'news/:id/edit', element: <NewsFormPage /> },
          { path: 'mosques', element: <MosquesPage /> },
          { path: 'mosques/new', element: <MosqueFormPage /> },
          { path: 'mosques/:id/edit', element: <MosqueFormPage /> },
          { path: 'files-manager', element: <FilesManagersPage /> },
          { path: 'files-manager/new', element: <FileFormPage /> },
          { path: 'files-manager/:id/edit', element: <FileFormPage /> },
          { path: 'files-manager/:id', element: <FileFormPage /> },
          { path: 'expensives', element: <ExpensivesPage /> },
          { path: 'expensives/new', element: <ExpensiveFormPage /> },
          { path: 'expensives/:id/edit', element: <ExpensiveFormPage /> },
          { path: 'expensives/:id', element: <ExpensiveFormPage /> },
          { path: 'teachers', element: <TeachersPage /> },
          { path: 'teachers/new', element: <TeacherFormPage /> },
          { path: 'teachers/:id/edit', element: <TeacherFormPage /> },
          { path: 'circles', element: <CirclesPage /> },
          { path: 'circles/new', element: <CircleFormPage /> },
          { path: 'circles/:id/edit', element: <CircleFormPage /> },
          { path: 'womans-activities', element: <WomansActivitiesPage /> },
          { path: 'current-students-plans', element: <CurrentStudentsPlansPage /> },
          { path: 'student-plans', element: <StudentPlanPage /> },
          { path: 'send-notes', element: <SendNotesPage /> },
          { path: 'send-notes/new', element: <SendNoteFormPage /> },
          { path: 'send-notes/:id/edit', element: <SendNoteFormPage /> },
          { path: 'teacher-salaries', element: <TeacherSalariesPage /> },
          { path: 'teacher-salaries/new', element: <TeacherSalaryFormPage /> },
          { path: 'teacher-salaries/report', element: <TeacherSalaryReportPage /> },
          { path: 'teacher-salaries/:id/edit', element: <TeacherSalaryFormPage /> },
          { path: 'attendance-report', element: <AttendanceReportPage /> },
          { path: 'teachers-attendance', element: <TeachersAttendancePage /> },
          { path: 'home', element: <HomePage /> },
          { path: 'othaimin-center', element: <OthaiminCenterPage /> },
          { path: 'students/new', element: <StudentPage /> },
          { path: 'students/:id/edit', element: <StudentPage /> },
          { path: 'memorization-revision-report', element: <MemorizationRevisionReportPage /> },
          { path: 'special-students-report', element: <SpecialStudentsReportPage /> },
          { path: 'students2', element: <Students2Page /> },
          { path: 'parent-panel-log-statistics', element: <ParentPanelLogStatisticsPage /> },
          { path: 'plan-levels', element: <PlanLevelPage /> },
          { path: 'qr-generator', element: <QRGeneratorPage /> },
          { path: 'statistics', element: <StatisticsPage /> },
          { path: 'whatsapp-sender', element: <WhatsappSenderPage /> },
          { path: 'whatsapp-pending', element: <WhatsappPendingPage /> },
          { path: 'whatsapp-config', element: <WhatsappPreConfiguredPage /> },
          { path: 'whatsapp-qr', element: <WhatsappQrPage /> },
          { path: 'push-notifications', element: <PushNotificationPage /> },
          { path: 'tests', element: <TestsReportPage /> },
          { path: 'settings', element: <SettingsPage /> },
          { path: 'integrations', element: <IntegrationsPage /> },
          { path: 'work-days', element: <WorkDaysPage /> },
            ],
          },
        ],
      },
    ],
  },
])
