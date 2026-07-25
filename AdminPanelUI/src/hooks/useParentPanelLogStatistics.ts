import { useQuery } from '@tanstack/react-query'
import { getParentPanelLogStatistics } from '@/services/parentPanelLogStatisticsService'
import type { ParentPanelLogStatisticsFilters } from '@/types/parentPanelLogStatistics'

export function useParentPanelLogStatistics(
  filters: ParentPanelLogStatisticsFilters | null,
) {
  return useQuery({
    queryKey: ['parent-panel-log-statistics', filters?.fromDate, filters?.toDate],
    queryFn: () => getParentPanelLogStatistics(filters!),
    enabled: Boolean(filters?.fromDate && filters?.toDate),
  })
}
