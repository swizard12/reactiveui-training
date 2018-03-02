<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
				xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
				xmlns="http://www.w3.org/1999/xhtml">



  <xsl:template match="/">
    <html>
      <head>
        <meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
        <title>Training Report</title>
      </head>
      <body>
        <h1>Training Report</h1>
        <h2>
          <xsl:value-of select="ArrayOfFnTEMPTRAINING_SELECTSResult/FnTEMPTRAINING_SELECTSResult[1]/SW_CODE"/>&#160;<xsl:value-of select="ArrayOfFnTEMPTRAINING_SELECTSResult/FnTEMPTRAINING_SELECTSResult[1]/SW_DESCRIPTION"/>
        </h2>
        <table border="1">
          <tbody>
            <xsl:for-each select="ArrayOfFnTEMPTRAINING_SELECTSResult/FnTEMPTRAINING_SELECTSResult">
              <tr>
                <td align="center" height="30" style="font-size:16pt">
                  <xsl:value-of select="EMP_FNAME"/>
                </td>
                <td align="center" height="30" style="font-size:16pt">
                  <xsl:value-of select="EMP_LNAME"/>
                </td>
                <xsl:choose>
                  <xsl:when test="(SW_LEVEL = 0) and (SW_CRITERIA = 0)">
                    <td align="center" bgcolor="grey" height="30" style="font-size:16pt"/>
                  </xsl:when>
                  <xsl:when test="(SW_LEVEL = 1) and (SW_CRITERIA = 0)">
                    <td align="center" bgcolor="green" height="30" style="font-size:16pt"/>
                  </xsl:when>
                  <xsl:when test="(SW_LEVEL = 2) and (SW_CRITERIA = 0)">
                    <td align="center" bgcolor="yellow" height="30" style="font-size:16pt"/>
                  </xsl:when>
                  <xsl:when test="(SW_LEVEL = 0) and (SW_CRITERIA = 1)">
                    <td align="center" bgcolor="red" height="30" style="font-size:16pt"/>
                  </xsl:when>
                  <xsl:when test="(SW_LEVEL = 1) and (SW_CRITERIA = 1)">
                    <td align="center" bgcolor="green" height="30" style="font-size:16pt"/>
                  </xsl:when>
                  <xsl:when test="(SW_LEVEL = 2) and (SW_CRITERIA = 1)">
                    <td align="center" bgcolor="yellow" height="30" style="font-size:16pt"/>
                  </xsl:when>
                </xsl:choose>
                <xsl:choose>
                  <xsl:when test="VALIDDATE='2000-01-01T00:00:00'">
                    <td align="center" height="30" style="font-size:16pt">

                    </td>
                  </xsl:when>
                  <xsl:when test="not(VALIDDATE='2000-01-01T00:00:00')">
                    <td align="center" height="30" style="font-size:16pt">
                      <xsl:value-of select="format-dateTime(VALIDDATE,'[D01] [MNn] [Y0001]')"/>
                    </td>
                  </xsl:when>
                </xsl:choose>
                <td align="center" height="30" style="font-size:16pt">
                  <xsl:value-of select="VALIDNOTE" />
                </td>
                <xsl:choose>
                  <xsl:when test="SW_LEVEL=0">
                    <td align="center" height="30" style="font-size:16pt">
                      <t>Full Training (1527)</t>
                    </td>
                  </xsl:when>
                  <xsl:when test="SW_LEVEL=1">
                    <td align="center" height="30" style="font-size:16pt">

                    </td>
                  </xsl:when>
                  <xsl:when test="SW_LEVEL=2">
                    <td align="center" height="30" style="font-size:16pt">
                      <t>Update Training (1527A)</t>
                    </td>
                  </xsl:when>
                </xsl:choose>
              </tr>
            </xsl:for-each>
          </tbody>
        </table>
      </body>
    </html>
  </xsl:template>

  <xsl:output method="html" encoding="utf-8"/>

</xsl:stylesheet>