import {createI18n} from 'vue-i18n';
import en from './i18n/en.json';
import zh from './i18n/zh.json';

const messages = {
    en,
    zh
}

const i18n = createI18n({
    locale: 'zh', // 默认语言
    fallbackLocale: 'zh', // 当缺少翻译包时，自动回退到zh
    messages
})

export default i18n;