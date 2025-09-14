CREATE TABLE users (
    id UUID PRIMARY KEY,
    telegramid BIGINT NOT NULL UNIQUE,
    username VARCHAR(100) NULL,
    firstname VARCHAR(100) NULL,
    lastname VARCHAR(100) NULL,
    createdat TIMESTAMP NOT NULL
);

-- Создание индекса для быстрого поиска по telegramid
CREATE INDEX idx_users_telegramid ON users(telegramid);



CREATE TABLE kpi (
    id BIGSERIAL PRIMARY KEY,
    shopid INTEGER NOT NULL,
    date DATE NOT NULL,
    position VARCHAR(255) NOT NULL,
    name VARCHAR(255) NOT NULL,
    localid VARCHAR(255) NULL,
    tnumber VARCHAR(255) NULL,
    cardtype VARCHAR(255) NULL,
    checks INTEGER NOT NULL DEFAULT 0,
    specialchecks INTEGER NOT NULL DEFAULT 0,
    
    -- Внешний ключ на таблицу shops
    CONSTRAINT fk_kpi_shops 
        FOREIGN KEY (shopid) 
        REFERENCES shops(id));
        --ON DELETE RESTRICT
        --ON UPDATE CASCADE);


CREATE TABLE shops (
    id SERIAL PRIMARY KEY,
    namebu VARCHAR(200) NOT NULL DEFAULT '',
    nameuu VARCHAR(200) NOT NULL DEFAULT '',
    nameqv VARCHAR(200) NOT NULL DEFAULT '',
    region VARCHAR(100) NOT NULL DEFAULT '',
    city VARCHAR(100) NOT NULL DEFAULT '',
    location TEXT NOT NULL DEFAULT '',
    category VARCHAR(50) NOT NULL DEFAULT '',
    rk NUMERIC(10, 2) NOT NULL DEFAULT 0,
    sn NUMERIC(10, 2) NOT NULL DEFAULT 0
    
);


-- FUNCTION: public.get_kpi(text, date)

-- DROP FUNCTION IF EXISTS public.get_kpi(text, date);

--CREATE OR REPLACE FUNCTION public.get_kpi(
--	p_name_pattern text,
--	p_date date)
--    RETURNS TABLE(shop text, category text, name text, total_checks bigint, sp_checks bigint, result numeric) 
--    LANGUAGE 'plpgsql'
--    COST 100
--    VOLATILE PARALLEL UNSAFE
--    ROWS 1000

--AS $BODY$
--BEGIN
--    RETURN QUERY
--    SELECT 
--        s.nameqv::TEXT AS shop,  -- Явное приведение к TEXT
--		s.category::TEXT as category, -- категория магазина
--        k.name::TEXT AS name,     -- Явное приведение к TEXT
--        SUM(k.checks)::BIGINT AS total_checks,
--        SUM(k.specialchecks)::BIGINT AS sp_checks,
--        ROUND(
--            CASE 
--                WHEN SUM(k.specialchecks) = 0 THEN 0
--                ELSE SUM(k.checks)::DECIMAL / NULLIF(SUM(k.specialchecks), 0)
--            END, 
--            2
--        ) AS result
--    FROM 
--        kpi k
--    INNER JOIN 
--        shops s ON k.shopid = s.id
--    WHERE 
--        LOWER(k.name) LIKE LOWER('%' || p_name_pattern || '%')
--        AND k.date >= DATE_TRUNC('month', p_date)
--        AND k.date < DATE_TRUNC('month', p_date) + INTERVAL '1 month'
--    GROUP BY 
--        s.nameqv, 
--        s.category,
--        k.name
--    ORDER BY 
--        s.nameqv, 
--        k.name;
--END;
--$BODY$;

--ALTER FUNCTION public.get_kpi(text, date)
--    OWNER TO postgres;

--COMMENT ON FUNCTION public.get_kpi(text, date)
--    IS 'Возвращает KPI показатели по фильтру имени за указанный месяц';

------------------------------

-- FUNCTION: public.get_kpi(text)

-- DROP FUNCTION IF EXISTS public.get_kpi(text);

--CREATE OR REPLACE FUNCTION public.get_kpi(
--	p_name_pattern text)
--    RETURNS TABLE(shop text, category text, name text, total_checks bigint, sp_checks bigint, result numeric) 
--    LANGUAGE 'plpgsql'
--    COST 100
--    VOLATILE PARALLEL UNSAFE
--    ROWS 1000

--AS $BODY$
--BEGIN
--    RETURN QUERY
--    SELECT 
--        s.nameqv::TEXT AS shop,  -- Явное приведение к TEXT
--		s.category::TEXT as category, -- категория магазина
--        k.name::TEXT AS name,     -- Явное приведение к TEXT
--        SUM(k.checks)::BIGINT AS total_checks,
--        SUM(k.specialchecks)::BIGINT AS sp_checks,
--        ROUND(
--            CASE 
--                WHEN SUM(k.specialchecks) = 0 THEN 0
--                ELSE SUM(k.checks)::DECIMAL / NULLIF(SUM(k.specialchecks), 0)
--            END, 
--            2
--        ) AS result
--    FROM 
--        kpi k
--    INNER JOIN 
--        shops s ON k.shopid = s.id
--    WHERE 
--        LOWER(k.name) LIKE LOWER('%' || p_name_pattern || '%')
--    GROUP BY 
--        s.nameqv, 
--        s.category,
--        k.name
--    ORDER BY 
--        s.nameqv, 
--        k.name;
--END;
--$BODY$;

--ALTER FUNCTION public.get_kpi(text)
--    OWNER TO postgres;

--COMMENT ON FUNCTION public.get_kpi(text)
--    IS 'Возвращает KPI показатели по фильтру имени за указанный месяц';


