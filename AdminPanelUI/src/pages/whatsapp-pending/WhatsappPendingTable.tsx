import type { WhatsappPendingMessage } from '@/types/whatsappPending'

interface WhatsappPendingTableProps {
  items: WhatsappPendingMessage[]
  selectedIds: Set<number>
  onToggle: (id: number) => void
  onToggleAll: (checked: boolean) => void
}

export function WhatsappPendingTable({
  items,
  selectedIds,
  onToggle,
  onToggleAll,
}: WhatsappPendingTableProps) {
  const allSelected = items.length > 0 && items.every((item) => selectedIds.has(item.id))

  return (
    <div className="overflow-x-auto rounded-xl border bg-white shadow-sm">
      <table className="w-full border-collapse text-sm">
        <thead>
          <tr className="bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] text-white">
            <th className="px-3 py-3 text-center">
              <input
                type="checkbox"
                checked={allSelected}
                onChange={(e) => onToggleAll(e.target.checked)}
                aria-label="تحديد الكل"
              />
            </th>
            <th className="px-3 py-3 text-center">رقم</th>
            <th className="px-3 py-3 text-center">رقم الجوال</th>
            <th className="px-3 py-3 text-right">الرسالة</th>
            <th className="px-3 py-3 text-center">صورة</th>
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.id} className="border-t hover:bg-slate-50">
              <td className="px-3 py-2 text-center">
                <input
                  type="checkbox"
                  checked={selectedIds.has(item.id)}
                  onChange={() => onToggle(item.id)}
                />
              </td>
              <td className="px-3 py-2 text-center">{item.id}</td>
              <td className="px-3 py-2 text-center">{item.mobile}</td>
              <td className="max-w-xs truncate px-3 py-2 text-right">{item.messagePreview}</td>
              <td className="px-3 py-2 text-center">{item.hasImage ? 'نعم' : 'لا'}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
