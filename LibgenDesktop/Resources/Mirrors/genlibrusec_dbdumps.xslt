<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
    <xsl:template match="/">
        <xsl:call-template name="file">
            <xsl:with-param name="filter">libgen_compact_</xsl:with-param>
        </xsl:call-template>
        <xsl:text>&#xa;</xsl:text>
        <xsl:call-template name="file">
            <xsl:with-param name="filter">fiction_</xsl:with-param>
        </xsl:call-template>
        <xsl:text>&#xa;</xsl:text>
        <xsl:call-template name="file">
            <xsl:with-param name="filter">scimag_</xsl:with-param>
        </xsl:call-template>
    </xsl:template>
    <xsl:template name="file">
        <xsl:param name="filter"/>
        <xsl:for-each select="(//a[contains(@href, $filter)])">
            <xsl:sort data-type="text" select="@href" order="descending"/>
            <xsl:if test="position() = 1">
                <xsl:value-of select="concat('http://gen.lib.rus.ec/dbdumps/',@href,'|',normalize-space(../../td[1]),'|',normalize-space(../../td[2]),'|',normalize-space(../../td[3]))"/>
            </xsl:if>
        </xsl:for-each>
    </xsl:template>
</xsl:stylesheet>