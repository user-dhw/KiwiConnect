import dayjs from 'dayjs'

/**
 * Format date string using dayjs
 * @param {string|number|Date} dateStr - Date string, timestamp, or Date object
 * @param {string} pattern - Date format pattern (default: 'YYYY-MM-DD HH:mm')
 * @returns {string} Formatted date string
 */
export const formatDate = (dateStr, pattern = 'YYYY-MM-DD HH:mm') => {
	return dayjs(dateStr).format(pattern)
}

export default formatDate
