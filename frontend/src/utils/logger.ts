// 记录运行中的日志
export const logger = {
  info: (...args: any[]) => {
    if (args.length == 1) console.info(args[0])
    else console.info(args)
  },
  warn: (...args: any[]) => {
    if (args.length == 1) console.warn(args[0])
    else console.warn(args)
  },
  error: (...args: any[]) => {
    if (args.length == 1) console.error(args[0])
    else console.error(args)
  }
}
