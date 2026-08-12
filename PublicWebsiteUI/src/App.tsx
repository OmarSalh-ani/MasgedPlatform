import { Route, Routes } from 'react-router-dom'
import { PublicLayout } from '@/layouts/PublicLayout'
import { ActivitiesPage } from '@/pages/ActivitiesPage/ActivitiesPage'
import { TipsPage } from '@/pages/TipsPage/TipsPage'
import { IndexPage } from '@/pages/IndexPage/IndexPage'
import { MosquesPage } from '@/pages/MosquesPage/MosquesPage'
import { NewsPage } from '@/pages/NewsPage/NewsPage'
import { ParentsFollowupPage } from '@/pages/parents-followup/ParentsFollowupPage'
import { RegisterSuccessPage } from '@/pages/RegisterSuccessPage/RegisterSuccessPage'
import { RegistrationPage } from '@/pages/RegistrationPage/RegistrationPage'
import { PrivacyPolicyPage } from '@/pages/PrivacyPolicyPage/PrivacyPolicyPage'

export default function App() {
  return (
    <Routes>
      <Route path="/parents-followup" element={<ParentsFollowupPage />} />
      <Route element={<PublicLayout />}>
        <Route path="/" element={<IndexPage />} />
        <Route path="/tips" element={<TipsPage />} />
        <Route path="/mosques" element={<MosquesPage />} />
        <Route path="/news" element={<NewsPage />} />
        <Route path="/activities" element={<ActivitiesPage />} />
        <Route path="/registration" element={<RegistrationPage />} />
        <Route path="/register-success" element={<RegisterSuccessPage />} />
        <Route path="/privacy-policy" element={<PrivacyPolicyPage locale="ar" />} />
        <Route path="/privacy-policy/en" element={<PrivacyPolicyPage locale="en" />} />
      </Route>
    </Routes>
  )
}
