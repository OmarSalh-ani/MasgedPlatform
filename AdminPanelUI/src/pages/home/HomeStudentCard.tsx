import { GraduationCap } from 'lucide-react'
import { resolveImageUrl } from '@/lib/resolveImageUrl'

import {

  HomeStudentActionButtons,

  HomeStudentSelectCheckbox,

  type HomeStudentItemProps,

} from '@/pages/home/HomeStudentCardParts'

import { HomeStudentCardDetails } from '@/pages/home/HomeStudentCardDetails'



export function HomeStudentCard(props: HomeStudentItemProps) {

  const { item, selected, canModify, onToggle, onDelete, onShowTests, onShowReviews } = props



  return (

    <article className="overflow-hidden rounded-xl border bg-white shadow-md transition hover:-translate-y-1 hover:shadow-lg">

      <div className="bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] px-5 py-6 text-center text-white">

        <div className="mx-auto mb-3 flex size-20 items-center justify-center overflow-hidden rounded-full border-4 border-white/30 bg-white/20">

          {item.studentImage ? (

            <img src={resolveImageUrl(item.studentImage)} alt={item.studentName} className="size-full object-cover" />

          ) : (

            <GraduationCap className="size-9" />

          )}

        </div>

        <h3 className="text-lg font-bold break-words">{item.studentName}</h3>

        <p className="text-sm opacity-80">رقم الطالب: {item.id}</p>

        {item.isSpecial === 'نعم' || item.isElite === 'نعم' ? (

          <div className="mt-2 flex flex-wrap justify-center gap-2">

            {item.isSpecial === 'نعم' ? (

              <span className="rounded-full bg-[#CBAC2D] px-2.5 py-0.5 text-xs font-bold">طالب مميز</span>

            ) : null}

            {item.isElite === 'نعم' ? (

              <span className="rounded-full bg-emerald-500 px-2.5 py-0.5 text-xs font-bold">طالب نخبة</span>

            ) : null}

          </div>

        ) : null}

      </div>



      <div className="space-y-2 p-5 text-sm">

        <HomeStudentCardDetails item={item} />



        <HomeStudentSelectCheckbox item={item} selected={selected} onToggle={onToggle} />



        <HomeStudentActionButtons

          item={item}

          canModify={canModify}

          onDelete={onDelete}

          onShowTests={onShowTests}

          onShowReviews={onShowReviews}

        />

      </div>

    </article>

  )

}

